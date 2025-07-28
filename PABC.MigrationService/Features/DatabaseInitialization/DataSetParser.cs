using System.Collections.Immutable;
using System.Text.Json;
using Json.Schema;
using Json.Schema.Generation;
using Json.Schema.Serialization;

namespace PABC.MigrationService.Features.DatabaseInitialization;

public interface IDatasetParser
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="JsonSchemaValidationException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    Task<DataSet> Parse(Stream stream, CancellationToken token);
}

public class JsonSchemaValidationException : Exception
{
    public required IReadOnlyList<JsonSchemaValidationError> Errors { get; init; }
}

public class JsonSchemaValidationError
{
    public required Json.Pointer.JsonPointer InstanceLocation { get; init; }
    public required IReadOnlyDictionary<string, string> Errors { get; init; }
}

public class DatasetParser : IDatasetParser
{
    private const string SchemaFilename = "dataset.schema.json";

    private static readonly JsonSerializerOptions s_jsonSerializerOptions = GetJsonSerializerOptions();

    public async Task<DataSet> Parse(Stream stream, CancellationToken token)
    {
        try
        {
            var result = await JsonSerializer.DeserializeAsync<DataSet>(stream, s_jsonSerializerOptions, token);
            return result!;
        }
        // the json schema validation errors are a bit hidden in the exception data. we collect them here and throw a custom exception
        catch (JsonException ex) when (ex.Data.Values.OfType<EvaluationResults>().FirstOrDefault() is { } results)
        {
            var errors = results.Details
                .Where(d => d.HasErrors)
                .Select(x => new JsonSchemaValidationError
                {
                    Errors = x.Errors!,
                    InstanceLocation = x.InstanceLocation
                })
                .ToList();
            throw new JsonSchemaValidationException { Errors = errors };
        }
    }

    public static async Task WriteSchemaToFile(CancellationToken token)
    {
        var schema = GenerateSchema();
        await using var file = File.OpenWrite(SchemaFilename);
        await JsonSerializer.SerializeAsync(file, schema, s_jsonSerializerOptions, cancellationToken: token);
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        var schema = GenerateSchema();
        ValidatingJsonConverter.MapType<DataSet>(schema);
        return new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
            Converters =
            {
                new ValidatingJsonConverter
                {
                    OutputFormat = OutputFormat.List,
                    RequireFormatValidation = true,
                }
            }
        };
    }

    private static JsonSchema GenerateSchema() => new JsonSchemaBuilder().FromType<DataSet>(new() { PropertyNameResolver = PropertyNameResolvers.CamelCase }).Build();
}
