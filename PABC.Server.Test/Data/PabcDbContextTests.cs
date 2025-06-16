using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PABC.Data.Entities;
using PABC.Server.Test.TestConfig;

namespace PABC.Server.Test.Data
{
    public class PabcDbContextTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>, IAsyncLifetime
    {
        [Fact]
        public async Task DuplicateMappingRowsAreImpossible()
        {
            var appRole = new ApplicationRole { Id = Guid.NewGuid(), Application = string.Empty, Name = string.Empty };
            var domain = new Domain { Id = Guid.NewGuid(), Description = string.Empty, EntityTypes = [], Name = string.Empty };
            var funcRole = new FunctionalRole { Id = Guid.NewGuid(), Name = string.Empty };

            var firstMapping = new Mapping { ApplicationRole = appRole, Domain = domain, FunctionalRole = funcRole, Id = Guid.NewGuid() };
            var secondMapping = new Mapping { ApplicationRole = appRole, Domain = domain, FunctionalRole = funcRole, Id = Guid.NewGuid() };

            fixture.DbContext.AddRange(appRole, domain, funcRole, firstMapping);
            await fixture.DbContext.SaveChangesAsync();

            fixture.DbContext.Add(secondMapping);

            await Assert.ThrowsAsync<DbUpdateException>(() => fixture.DbContext.SaveChangesAsync());
        }

        [Fact]
        public async Task DuplicateApplicationRolesAreImpossible()
        {
            var appRole = new ApplicationRole { Id = Guid.NewGuid(), Application = string.Empty, Name = string.Empty };
            var appRole2 = new ApplicationRole { Id = Guid.NewGuid(), Application = string.Empty, Name = string.Empty };

            fixture.DbContext.Add(appRole);
            await fixture.DbContext.SaveChangesAsync();

            fixture.DbContext.Add(appRole2);

            await Assert.ThrowsAsync<DbUpdateException>(() => fixture.DbContext.SaveChangesAsync());
        }

        [Fact]
        public async Task DuplicateFunctionalRolesAreImpossible()
        {
            var funcRole = new FunctionalRole { Id = Guid.NewGuid(), Name = string.Empty };
            var funcRole2 = new FunctionalRole { Id = Guid.NewGuid(), Name = string.Empty };

            fixture.DbContext.Add(funcRole);
            await fixture.DbContext.SaveChangesAsync();

            fixture.DbContext.Add(funcRole2);

            await Assert.ThrowsAsync<DbUpdateException>(() => fixture.DbContext.SaveChangesAsync());
        }

        [Fact]
        public async Task DuplicateDomainsAreImpossible()
        {
            var domain = new Domain { Id = Guid.NewGuid(), Description = string.Empty, EntityTypes = [], Name = string.Empty };
            var domain2 = new Domain { Id = Guid.NewGuid(), Description = string.Empty, EntityTypes = [], Name = string.Empty };

            fixture.DbContext.Add(domain);
            await fixture.DbContext.SaveChangesAsync();

            fixture.DbContext.Add(domain2);

            await Assert.ThrowsAsync<DbUpdateException>(() => fixture.DbContext.SaveChangesAsync());
        }

        [Fact]
        public async Task DuplicateEntityTypesAreImpossible()
        {
            var entityType1 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = Guid.NewGuid().ToString(), Name = string.Empty };
            var entityType2 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = entityType1.EntityTypeId, Type = entityType1.Type, Name = string.Empty };

            fixture.DbContext.AddRange(entityType1);
            await fixture.DbContext.SaveChangesAsync();

            fixture.DbContext.Add(entityType2);

            await Assert.ThrowsAsync<DbUpdateException>(() => fixture.DbContext.SaveChangesAsync());
        }

        [Fact]
        public void TestIfWeForgotToAddAMigration()
        {
            var context = fixture.DbContext;
            var migrationsAssembly = context.GetService<IMigrationsAssembly>();

            var snapshotModel = migrationsAssembly.ModelSnapshot?.Model;

            if (snapshotModel is IMutableModel mutableModel)
            {
                snapshotModel = mutableModel.FinalizeModel();
            }

            if (snapshotModel != null)
            {
                snapshotModel = context.GetService<IModelRuntimeInitializer>().Initialize(snapshotModel);
            }

            var differ = context.GetService<IMigrationsModelDiffer>();

            var differences = differ.GetDifferences(
                snapshotModel?.GetRelationalModel(),
                context.GetService<IDesignTimeModel>().Model.GetRelationalModel());

            Assert.Equal(0, differences.Count);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await ClearDatabaseAsync();
        }

        private async Task ClearDatabaseAsync()
        {
            fixture.DbContext.ChangeTracker.Clear();
            var migrator = fixture.DbContext.Database.GetInfrastructure().GetRequiredService<IMigrator>();
            await migrator.MigrateAsync("0");
            await migrator.MigrateAsync();
        }
    }
}
