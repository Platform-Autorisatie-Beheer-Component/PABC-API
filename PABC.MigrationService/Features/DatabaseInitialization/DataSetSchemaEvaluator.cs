using Corvus.Text.Json;

namespace PABC.MigrationService.Features.DatabaseInitialization;

[JsonSchemaTypeGenerator("../../dataset.schema.json", EmitEvaluator = true)]
public readonly partial struct DataSetSchema;
