using Microsoft.EntityFrameworkCore;
using PABC.Data.Entities;
using PABC.MigrationService.Features.DatabaseInitialization;
using PABC.Server.Test.TestConfig;

namespace PABC.Server.Test.MigrationService.Features
{
    public class DatabaseInitializerTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>, IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            await ClearDatabaseAsync();
            await fixture.DbContext.AddRangeAsync([
                new FunctionalRole{ Id = Guid.NewGuid(), Name = Guid.NewGuid().ToString() }
            ]);
            await fixture.DbContext.SaveChangesAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private async Task ClearDatabaseAsync()
        {
            var dbContext = fixture.DbContext;
            await dbContext.ApplicationRoles.ExecuteDeleteAsync();
            await dbContext.FunctionalRoles.ExecuteDeleteAsync();
            await dbContext.Domains.ExecuteDeleteAsync();
            await dbContext.EntityTypes.ExecuteDeleteAsync();
            await dbContext.Mappings.ExecuteDeleteAsync();
        }

        [Fact]
        public async Task If_dataSet_is_null_no_data_is_deleted()
        {
            var initializer = new DatabaseInitializer(fixture.DbContext);

            await initializer.Initialize(null, CancellationToken.None);

            var functionalRoleCount = await fixture.DbContext.FunctionalRoles.CountAsync();
            Assert.Equal(1, functionalRoleCount);
        }

        [Fact]
        public async Task If_dataSet_is_empty_all_data_is_deleted()
        {
            var initializer = new DatabaseInitializer(fixture.DbContext);

            await initializer.Initialize(new DataSet { ApplicationRoles = [], Domains = [], EntityTypes = [], FunctionalRoles = [], Mappings = [] }, CancellationToken.None);

            var functionalRoleCount = await fixture.DbContext.FunctionalRoles.CountAsync();
            Assert.Equal(0, functionalRoleCount);
        }
    }
}
