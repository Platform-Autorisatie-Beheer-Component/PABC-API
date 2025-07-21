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

internal class DatasetParser(ILogger<DatasetParser> logger) : IDatasetParser
{
    private const string SchemaFilename = "dataset.schema.json";

    // Configures the JSON serializer to validate against JSON Schema during deserialization
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new ValidatingJsonConverter
            {
                OutputFormat = OutputFormat.List,
                RequireFormatValidation = true,
            }
        }
    };

    public static readonly JsonSchema Schema = JsonSchema.FromFile(SchemaFilename);


    public async Task<DataSet> Parse(Stream stream, CancellationToken token)
    {
        try
        {
            var result = await JsonSerializer.DeserializeAsync<DataSet>(stream, s_jsonOptions, token);
            return result!;
        }
        catch (JsonException ex)
        {
            var results = ex.Data.OfType<DictionaryEntry>().Select(x => x.Value).OfType<EvaluationResults>().FirstOrDefault();
            if (results != null)
            {
                var errors = results.Details.Where(d => d.HasErrors).Select(x => new { x.Errors, x.InstanceLocation }).ToList();
                logger.LogError("Validation failed: {@Errors}", JsonSerializer.Serialize(errors, s_jsonOptions));
            }
            throw;
        }
    }

    public static async Task WriteSchemaToFile(CancellationToken token)
    {
        var schema = new JsonSchemaBuilder().FromType<DataSet>(new() { PropertyNameResolver = PropertyNameResolvers.CamelCase }).Build();
        await using var file = File.OpenWrite(SchemaFilename);
        await JsonSerializer.SerializeAsync(file, schema, s_jsonOptions, token);
    }
}
