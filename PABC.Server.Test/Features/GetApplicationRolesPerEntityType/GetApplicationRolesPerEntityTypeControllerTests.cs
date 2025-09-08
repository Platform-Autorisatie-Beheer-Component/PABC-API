using PABC.Data;
using PABC.Data.Entities;
using PABC.Server.Features.GetApplicationRolesPerEntityType;
using PABC.Server.Test.TestConfig;

namespace PABC.Server.Test.Features.GetApplicationRolesPerEntityType
{
    public class GetApplicationRolesPerEntityTypeControllerTests(PostgresFixture fixture)
        : IClassFixture<PostgresFixture>, IAsyncLifetime
    {
        private static readonly string ValidFunctionalRole = "FunctionalRoleA";
        private static readonly string InvalidFunctionalRole = "NonExistingRole";
        private static readonly string ApplicationRoleName = "AppRoleA";
        private static readonly string ApplicationName = "AppA";
        private static readonly string EntityTypeName = "EntityTypeA";
        private static readonly string EntityTypeType = "TypeA";
        private static readonly string DomainSpecificAppRoleName = "DomainSpecificRole";
        private static readonly string AllEntityTypesAppRoleName = "AllEntityTypesRole";
        private static readonly string SecondEntityTypeName = "EntityTypeB";
        private static readonly string SecondEntityTypeType = "TypeB";
        private readonly PabcDbContext _dbContext = fixture.DbContext;

        public async Task InitializeAsync()
        {
            await ClearDatabaseAsync();
            await SeedTestDataAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        private GetApplicationRolesPerEntityTypeController CreateController()
        {
            return new GetApplicationRolesPerEntityTypeController(_dbContext);
        }

        private GetApplicationRolesRequest CreateRequest(params string[] roles)
        {
            return new GetApplicationRolesRequest { FunctionalRoleNames = roles };
        }

        private async Task ClearDatabaseAsync()
        {
            _dbContext.ChangeTracker.Clear();
            _dbContext.Mappings.RemoveRange(_dbContext.Mappings);
            _dbContext.Domains.RemoveRange(_dbContext.Domains);
            _dbContext.EntityTypes.RemoveRange(_dbContext.EntityTypes);
            _dbContext.FunctionalRoles.RemoveRange(_dbContext.FunctionalRoles);
            _dbContext.ApplicationRoles.RemoveRange(_dbContext.ApplicationRoles);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedTestDataAsync()
        {
            var functionalRole = new FunctionalRole { Id = Guid.NewGuid(), Name = ValidFunctionalRole };

            var applicationRole = new ApplicationRole
            {
                Id = Guid.NewGuid(), Name = ApplicationRoleName, Application = ApplicationName
            };
            var domainSpecificAppRole = new ApplicationRole
            {
                Id = Guid.NewGuid(), Name = DomainSpecificAppRoleName, Application = ApplicationName
            };
            var allEntityTypesAppRole = new ApplicationRole
            {
                Id = Guid.NewGuid(), Name = AllEntityTypesAppRoleName, Application = ApplicationName
            };

            var entityTypeA = new EntityType
            {
                Id = Guid.NewGuid(),
                EntityTypeId = Guid.NewGuid().ToString(),
                Name = EntityTypeName,
                Type = EntityTypeType
            };
            var entityTypeB = new EntityType
            {
                Id = Guid.NewGuid(),
                EntityTypeId = Guid.NewGuid().ToString(),
                Name = SecondEntityTypeName,
                Type = SecondEntityTypeType
            };

            var domain1 = new Domain { Id = Guid.NewGuid(), Description = string.Empty, Name = "Domain 1" };
            domain1.EntityTypes.Add(entityTypeA);

            var domain2 = new Domain { Id = Guid.NewGuid(), Description = string.Empty, Name = "Domain 2" };
            domain2.EntityTypes.Add(entityTypeA);
            domain2.EntityTypes.Add(entityTypeB);

            var mappings = new List<Mapping>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FunctionalRoleId = functionalRole.Id,
                    ApplicationRoleId = applicationRole.Id,
                    DomainId = domain1.Id,
                    IsAllEntityTypes = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FunctionalRoleId = functionalRole.Id,
                    ApplicationRoleId = applicationRole.Id,
                    DomainId = domain2.Id,
                    IsAllEntityTypes = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FunctionalRoleId = functionalRole.Id,
                    ApplicationRoleId = domainSpecificAppRole.Id,
                    DomainId = domain1.Id,
                    IsAllEntityTypes = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FunctionalRoleId = functionalRole.Id,
                    ApplicationRoleId = allEntityTypesAppRole.Id,
                    IsAllEntityTypes = true
                }
            };

            _dbContext.AddRange(functionalRole, applicationRole, domainSpecificAppRole, allEntityTypesAppRole);
            _dbContext.AddRange(entityTypeA, entityTypeB, domain1, domain2);
            _dbContext.AddRange(mappings);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task Post_ReturnsExpectedData_ForValidFunctionalRole()
        {
            var result = await CreateController().Post(CreateRequest(ValidFunctionalRole));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            var entityTypeResult = response.Results.FirstOrDefault(r => r.EntityType.Name == EntityTypeName);
            Assert.NotNull(entityTypeResult);

            Assert.Contains(entityTypeResult.ApplicationRoles, role =>
                role.Name == ApplicationRoleName && role.Application == ApplicationName);
        }

        [Fact]
        public async Task Post_IgnoresUnknownFunctionRoles_WhenFunctionalRoleDoesNotExist()
        {
            var result = await CreateController().Post(CreateRequest(ValidFunctionalRole, InvalidFunctionalRole));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            Assert.NotEmpty(response.Results);

            var entityTypeResult = response.Results.FirstOrDefault(r => r.EntityType.Name == EntityTypeName);
            Assert.NotNull(entityTypeResult);

            Assert.Contains(entityTypeResult.ApplicationRoles, role =>
                role.Name == ApplicationRoleName && role.Application == ApplicationName);
        }

        [Fact]
        public async Task Post_ReturnsEmpty_WhenFunctionalRoleDoesNotExist()
        {
            var result = await CreateController().Post(CreateRequest(InvalidFunctionalRole));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);
            Assert.Empty(response.Results);
        }


        [Fact]
        public async Task Post_ReturnsEmpty_WhenFunctionalRoleNamesIsEmpty()
        {
            var result = await CreateController().Post(CreateRequest());
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);
            Assert.Empty(response.Results);
        }

        [Fact]
        public async Task Post_HandlesDuplicateFunctionalRoleNames_WithoutDuplication()
        {
            var result = await CreateController().Post(CreateRequest(ValidFunctionalRole, ValidFunctionalRole));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            var entityTypeResult = response.Results.FirstOrDefault(r => r.EntityType.Name == EntityTypeName);
            Assert.NotNull(entityTypeResult);


            var appRoles = entityTypeResult.ApplicationRoles.Where(role =>
                role.Name == ApplicationRoleName && role.Application == ApplicationName).ToList();
            Assert.Single(appRoles);
        }

        [Fact]
        public async Task Post_HandlesDuplicateMappings_WithoutDuplicateApplicationRoles()
        {
            var result = await CreateController().Post(CreateRequest(ValidFunctionalRole));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            var entityTypeResult = response.Results.FirstOrDefault(r => r.EntityType.Name == EntityTypeName);
            Assert.NotNull(entityTypeResult);

            var appRoles = entityTypeResult.ApplicationRoles.Where(role =>
                role.Name == ApplicationRoleName && role.Application == ApplicationName).ToList();
            Assert.Single(appRoles);
        }

        [Fact]
        public async Task Post_ReturnsAllEntityTypes_WhenMappingIsAllEntityTypes()
        {
            var result = await CreateController().Post(CreateRequest(ValidFunctionalRole));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            Assert.Equal(2, response.Results.Count);

            foreach (var entityTypeResult in response.Results)
            {
                Assert.Contains(entityTypeResult.ApplicationRoles, role => role.Name == AllEntityTypesAppRoleName);
            }
        }

        [Fact]
        public async Task Post_ReturnsDomainSpecificEntityTypes_WhenMappingIsDomainSpecific()
        {
            var result = await CreateController().Post(CreateRequest(ValidFunctionalRole));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            var entityTypeAResult = response.Results.FirstOrDefault(r => r.EntityType.Name == EntityTypeName);
            Assert.NotNull(entityTypeAResult);
            Assert.Contains(entityTypeAResult.ApplicationRoles, role => role.Name == DomainSpecificAppRoleName);

            var entityTypeBResult = response.Results.FirstOrDefault(r => r.EntityType.Name == SecondEntityTypeName);
            Assert.NotNull(entityTypeBResult);
            Assert.DoesNotContain(entityTypeBResult.ApplicationRoles, role => role.Name == DomainSpecificAppRoleName);
        }

        [Fact]
        public async Task Post_CombinesBothMappingTypes_WhenBothExist()
        {
            var result = await CreateController().Post(CreateRequest(ValidFunctionalRole));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            var entityTypeAResult = response.Results.FirstOrDefault(r => r.EntityType.Name == EntityTypeName);
            Assert.NotNull(entityTypeAResult);

            Assert.Contains(entityTypeAResult.ApplicationRoles, role => role.Name == DomainSpecificAppRoleName);
            Assert.Contains(entityTypeAResult.ApplicationRoles, role => role.Name == AllEntityTypesAppRoleName);

            var entityTypeBResult = response.Results.FirstOrDefault(r => r.EntityType.Name == SecondEntityTypeName);
            Assert.NotNull(entityTypeBResult);
            Assert.Contains(entityTypeBResult.ApplicationRoles, role => role.Name == AllEntityTypesAppRoleName);
            Assert.DoesNotContain(entityTypeBResult.ApplicationRoles, role => role.Name == DomainSpecificAppRoleName);
        }
    }
}
