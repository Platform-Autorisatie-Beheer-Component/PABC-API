using System.Collections;
using System.Text.Json;
using Json.Schema;
using Json.Schema.Generation;
using Json.Schema.Serialization;

namespace PABC.MigrationService.Features.DatabaseInitialization;

public interface IDatasetParser
{
    Task<DataSet> Parse(Stream stream, CancellationToken token);
}

public class DatasetParser(ILogger<DatasetParser> logger) : IDatasetParser
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
        catch (JsonException ex)
        {
            var results = ex.Data.OfType<DictionaryEntry>().Select(x => x.Value).OfType<EvaluationResults>().FirstOrDefault();
            if (results != null)
            {
                // because the details of the validation are hidden in the Data dictionary, we log them here before re-throwing
                var errors = results.Details.Where(d => d.HasErrors).Select(x => new { x.Errors, x.InstanceLocation }).ToList();
                logger.LogError("Validation failed: {@Errors}", JsonSerializer.Serialize(errors, s_jsonSerializerOptions));
            }
            throw;
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
