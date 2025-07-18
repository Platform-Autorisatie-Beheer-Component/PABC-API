using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.MigrationService;

//https://learn.microsoft.com/en-us/dotnet/aspire/database/ef-core-migrations
public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);
        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PabcDbContext>();
            await RunWithinTransactionAsync(dbContext, async () =>
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
                await LoadDatasetAsync(dbContext, cancellationToken);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    /// <summary>
    /// If we use a transaction directly, we get this exception:<br/>
    /// The configured execution strategy 'NpgsqlRetryingExecutionStrategy' does not support user-initiated transactions. Use the execution strategy returned by 'DbContext.Database.CreateExecutionStrategy()' to execute all the operations in the transaction as a retriable unit.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
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

    private async Task LoadDatasetAsync(PabcDbContext dbContext, CancellationToken cancellationToken)
    {
        var dataSetPath = configuration["JSON_DATASET_PATH"];
        if (string.IsNullOrWhiteSpace(dataSetPath)) return;

        await using var file = File.OpenRead(dataSetPath);
        var dataSet = await JsonSerializer.DeserializeAsync<DataSet>(file, s_jsonOptions, cancellationToken);

        var newEntities = MapToEntities(dataSet!);

        await dbContext.ApplicationRoles.ExecuteDeleteAsync(cancellationToken);
        await dbContext.FunctionalRoles.ExecuteDeleteAsync(cancellationToken);
        await dbContext.Domains.ExecuteDeleteAsync(cancellationToken);
        await dbContext.EntityTypes.ExecuteDeleteAsync(cancellationToken);
        await dbContext.Mappings.ExecuteDeleteAsync(cancellationToken);

        await dbContext.AddRangeAsync(newEntities, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

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
