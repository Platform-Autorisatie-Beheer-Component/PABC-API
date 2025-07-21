using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.MigrationService.Features.DatabaseInitialization
{
    public interface IDatabaseInitializer
    {
        Task Initialize(DataSet? dataset, CancellationToken cancellationToken);
    }

    public class DatabaseInitializer(PabcDbContext dbContext) : IDatabaseInitializer
    {
        public Task Initialize(DataSet? dataSet, CancellationToken cancellationToken) => RunWithinTransactionAsync(dbContext, async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            if (dataSet != null)
            {
                await UpserDataSet(dataSet, cancellationToken);
            }
        }, cancellationToken);

        private async Task UpserDataSet(DataSet dataSet, CancellationToken cancellationToken)
        {
            var newEntities = DataSetMapper.MapToEntities(dataSet);

            // Clear existing entries from relevant tables
            await dbContext.ApplicationRoles.ExecuteDeleteAsync(cancellationToken);
            await dbContext.FunctionalRoles.ExecuteDeleteAsync(cancellationToken);
            await dbContext.Domains.ExecuteDeleteAsync(cancellationToken);
            await dbContext.EntityTypes.ExecuteDeleteAsync(cancellationToken);
            await dbContext.Mappings.ExecuteDeleteAsync(cancellationToken);
            await dbContext.AddRangeAsync(newEntities, cancellationToken);

            // Insert new records
            await dbContext.SaveChangesAsync(cancellationToken);
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
    }
}
