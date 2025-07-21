using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using Json.Schema;
using Json.Schema.Serialization;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.MigrationService;

/// <summary>
/// A background worker that handles EF Core migrations and loads initial data from a JSON dataset.
/// If migrations fail, the process exits with a non-zero code to prevent application startup.
/// See https://learn.microsoft.com/en-us/dotnet/aspire/database/ef-core-migrations
/// </summary>
public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    // Used for distributed tracing and diagnostics
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    // Configures the JSON serializer to validate against JSON Schema during deserialization
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new ValidatingJsonConverter
            {
                OutputFormat = Json.Schema.OutputFormat.List,
                RequireFormatValidation = true,
            }
        }
    };

    /// <summary>
    /// Entry point of the background worker.
    /// Runs the database migration and loads the dataset within a transaction.
    /// If any error occurs, logs the exception and halts the application startup.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Worker>();
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);
        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PabcDbContext>();

            // Run migrations and dataset loading within a retry-safe transaction
            await RunWithinTransactionAsync(dbContext, async () =>
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
                await LoadDatasetAsync(dbContext, cancellationToken);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);

            // Extract and log detailed JSON Schema validation errors if present
            // JsonSchema.Net puts the validation errors in the data of the exception...
            var results = ex.Data.OfType<DictionaryEntry>().Select(x => x.Value).OfType<EvaluationResults>().FirstOrDefault();
            if (results != null)
            {
                var errors = results.Details.Where(d => d.HasErrors).Select(x => new { x.Errors, x.InstanceLocation }).ToList();
                logger.LogCritical("Validation failed: {@Errors}", JsonSerializer.Serialize(errors, s_jsonOptions));
            }
            logger.LogCritical(ex, "Migrations failed");

            // Exit process with error code to block PABC.Server startup
            Environment.ExitCode = 1;
        }
        finally
        {
            // Gracefully shut down the host application once work is complete
            hostApplicationLifetime.StopApplication();
        }
    }

    /// <summary>
    /// Executes operations within a database transaction using EF Core's retrying execution strategy.
    /// This is required for providers like Npgsql that manage retries internally.
    /// </summary>
    private static async Task RunWithinTransactionAsync(DbContext dbContext, Func<Task> handler, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await handler();
            await transaction.CommitAsync(cancellationToken);
        });
    }

    /// <summary>
    /// Loads and deserializes a dataset from a JSON file (path from config),
    /// clears existing DB entries, and inserts new ones.
    /// </summary>
    private async Task LoadDatasetAsync(PabcDbContext dbContext, CancellationToken cancellationToken)
    {
        var dataSetPath = configuration["JSON_DATASET_PATH"];
        if (string.IsNullOrWhiteSpace(dataSetPath)) return;

        await using var file = File.OpenRead(dataSetPath);

        // Deserialize and validate the dataset using JSON Schema
        var dataSet = await JsonSerializer.DeserializeAsync<DataSet>(file, s_jsonOptions, cancellationToken);

        var newEntities = MapToEntities(dataSet!);

        // Clear existing entries from relevant tables
        await dbContext.ApplicationRoles.ExecuteDeleteAsync(cancellationToken);
        await dbContext.FunctionalRoles.ExecuteDeleteAsync(cancellationToken);
        await dbContext.Domains.ExecuteDeleteAsync(cancellationToken);
        await dbContext.EntityTypes.ExecuteDeleteAsync(cancellationToken);
        await dbContext.Mappings.ExecuteDeleteAsync(cancellationToken);

        // Insert new records
        await dbContext.AddRangeAsync(newEntities, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Maps the deserialized JSON dataset into EF Core entity objects for persistence.
    /// Handles relationships between domains, roles, and mappings.
    /// </summary>
    private static IEnumerable<object> MapToEntities(DataSet dataSet)
    {
        var functionalRoles = dataSet.FunctionalRoles
            .Select(functionalRole => new FunctionalRole
            {
                Id = functionalRole.Id,
                Name = functionalRole.Name
            })
            .ToList();

        var appRoles = dataSet.ApplicationRoles
            .Select(appRole => new ApplicationRole
            {
                Id = appRole.Id,
                Application = appRole.Application,
                Name = appRole.Name
            })
            .ToList();

        var entityTypes = dataSet.EntityTypes
            .Select(entityType => new EntityType
            {
                Id = entityType.Id,
                Name = entityType.Name,
                EntityTypeId = entityType.EntityTypeId,
                Type = entityType.Type,
                Uri = entityType.Uri
            })
            .ToList();

        var domains = dataSet.Domains
            .Select(domain => new Domain
            {
                Id = domain.Id,
                Name = domain.Name,
                Description = domain.Description,
                EntityTypes = [.. entityTypes.Where(e => domain.EntityTypes.Contains(e.Id))]
            })
            .ToList();

        var mappings = dataSet.Mappings
            .Select(mapping => new Mapping
            {
                Id = mapping.Id,
                ApplicationRole = appRoles.Single(a => a.Id == mapping.ApplicationRole),
                FunctionalRole = functionalRoles.Single(f => f.Id == mapping.FunctionalRole),
                Domain = domains.Single(d => d.Id == mapping.Domain),
            })
            .ToList();

        return functionalRoles
            .Cast<object>()
            .Concat(appRoles)
            .Concat(entityTypes)
            .Concat(domains)
            .Concat(mappings);
    }
}
