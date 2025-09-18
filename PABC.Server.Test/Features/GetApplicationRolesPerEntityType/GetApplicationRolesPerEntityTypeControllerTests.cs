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
        private static readonly string NoEntityTypesAppRoleName = "NoEntityTypesAppRoleName";
        private static readonly string SecondEntityTypeName = "EntityTypeB";
        private static readonly string SecondEntityTypeType = "TypeB";
        private readonly PabcDbContext _dbContext = fixture.DbContext;

        public async Task InitializeAsync()
        {
            await ClearDatabaseAsync();
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

        [Fact]
        public async Task Post_ReturnsExpectedData_ForValidFunctionalRole()
        {
            var someOtherDomain = RandomDomain(RandomEntityType());
            _dbContext.Add(someOtherDomain);
            _dbContext.SaveChanges();

            var theExpectedEntityType = RandomEntityType();
            var theExpectedDomain = RandomDomain(theExpectedEntityType);

            var mapping = InsertTestMapping(RandomFunctionalRole(), RandomApplicationRole(), theExpectedDomain, isAllEntityTypes: false);

            var result = await CreateController().Post(CreateRequest(mapping.FunctionalRole.Name));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            var singleResult = Assert.Single(response.Results);
            Assert.NotNull(singleResult.EntityType);
            Assert.Equal(theExpectedEntityType.Name, singleResult.EntityType.Name);
            var singleAppRole = Assert.Single(singleResult.ApplicationRoles);
            Assert.Equal(mapping.ApplicationRole.Name, singleAppRole.Name);
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
            // ARRANGE
            var entity1 = RandomEntityType();
            var entity2 = RandomEntityType();

            var domain1 = RandomDomain(entity1);
            var domain2 = RandomDomain(entity2);

            _dbContext.AddRange(domain1, domain2);
            _dbContext.SaveChanges();

            var mapping = InsertTestMapping(RandomFunctionalRole(), RandomApplicationRole(), domain: null, isAllEntityTypes: true);


            // ACT
            var result = await CreateController().Post(CreateRequest(mapping.FunctionalRole.Name));
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);


            // ASSERT
            Assert.Equal(2, response.Results.Count);

            Assert.Collection(response.Results, 
                // from domain 1
                result =>
                {
                    Assert.Equal(entity1.Name, result.EntityType!.Name);
                    var singleAppRole = Assert.Single(result.ApplicationRoles);
                    Assert.Equal(mapping.ApplicationRole.Name, singleAppRole.Name);
                },
                // from domain 2
                result =>
                {
                    Assert.Equal(entity2.Name, result.EntityType!.Name);
                    var singleAppRole = Assert.Single(result.ApplicationRoles);
                    Assert.Equal(mapping.ApplicationRole.Name, singleAppRole.Name);
                });
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

        [Fact]
        public async Task Post_ReturnsNullEntityType_WhenMappingIsNotAllEntityTypesAndDomainIsNull()
        {
            // ARRANGE
            // insert some domain data to make sure it's not included in the result
            var domain = RandomDomain(RandomEntityType());
            _dbContext.Add(domain);
            _dbContext.SaveChanges();

            var mapping = InsertTestMapping(RandomFunctionalRole(), RandomApplicationRole(), domain: null, isAllEntityTypes: false);

            // ACT
            var result = await CreateController().Post(CreateRequest(mapping.FunctionalRole.Name));

            // ASSERT
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);
            var singleResult = Assert.Single(response.Results);
            
            Assert.Null(singleResult.EntityType);
            
            var singleAppRole = Assert.Single(singleResult.ApplicationRoles);
            Assert.Equal(mapping.ApplicationRole.Name, singleAppRole.Name);
        }

        private Mapping InsertTestMapping(FunctionalRole functionalRole, ApplicationRole applicationRole, Domain? domain, bool isAllEntityTypes)
        {
            var mapping = new Mapping 
            { 
                ApplicationRoleId = applicationRole.Id, 
                FunctionalRoleId = functionalRole.Id, 
                IsAllEntityTypes = isAllEntityTypes,
                DomainId = domain?.Id,
                Id = Guid.NewGuid(),  
            };
            if (domain != null)
            {
                _dbContext.Add(domain);
            }
            _dbContext.AddRange(functionalRole, applicationRole, mapping);
            _dbContext.SaveChanges();
            
            return mapping;
        }

        private class DomainTestModel
        {
            public required int EntityTypeCount { get; init; }
        }

        private static string RandomString() => Guid.NewGuid().ToString();

        private static Domain RandomDomain(params EntityType[] entityTypes)
        {
            var domain = new Domain() { Description = RandomString(), Id = Guid.NewGuid(), Name = RandomString() };
            domain.EntityTypes.AddRange(entityTypes);
            return domain;
        }

        private static EntityType RandomEntityType() => new()
        {
            EntityTypeId = RandomString(),
            Name = RandomString(),
            Type = RandomString(),
            Id = Guid.NewGuid()
        };

        private static FunctionalRole RandomFunctionalRole() => new()
        {
            Id = Guid.NewGuid(),
            Name = RandomString()
        };

        private static ApplicationRole RandomApplicationRole() => new()
        {
            Id = Guid.NewGuid(),
            Application = RandomString(),
            Name = RandomString()
        };
    }
}
