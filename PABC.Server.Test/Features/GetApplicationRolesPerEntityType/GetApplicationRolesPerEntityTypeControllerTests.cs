using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;
using PABC.Server.Features.GetApplicationRolesPerEntityType;
using PABC.Server.Test.TestConfig;

namespace PABC.Server.Test.Features.GetApplicationRolesPerEntityType
{
    public class GetApplicationRolesPerEntityTypeControllerTests(PostgresFixture fixture)
        : IClassFixture<PostgresFixture>, IAsyncLifetime
    {
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
            // ARRANGE
            var someOtherDomain = RandomDomain(RandomEntityType());
            _dbContext.Add(someOtherDomain);
            _dbContext.SaveChanges();

            var theExpectedEntityType = RandomEntityType();
            var theExpectedDomain = RandomDomain(theExpectedEntityType);

            var mapping = InsertTestMapping(RandomFunctionalRole(), RandomApplicationRole(), theExpectedDomain, isAllEntityTypes: false);

            // ACT
            var result = await CreateController().Post(CreateRequest(mapping.FunctionalRole.Name));
            
            
            // ASSERT
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
            // ARRANGE
            var theExpectedEntityType = RandomEntityType();
            var theExpectedDomain = RandomDomain(theExpectedEntityType);

            var mapping = InsertTestMapping(RandomFunctionalRole(), RandomApplicationRole(), theExpectedDomain, isAllEntityTypes: false);

            // ACT
            var result = await CreateController().Post(CreateRequest(mapping.FunctionalRole.Name, RandomString(), RandomString()));
            
            
            // ASSERT
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            var singleResult = Assert.Single(response.Results);
            Assert.NotNull(singleResult.EntityType);
            Assert.Equal(theExpectedEntityType.Name, singleResult.EntityType.Name);
            var singleAppRole = Assert.Single(singleResult.ApplicationRoles);
            Assert.Equal(mapping.ApplicationRole.Name, singleAppRole.Name);
        }

        [Fact]
        public async Task Post_ReturnsEmpty_WhenFunctionalRoleDoesNotExist()
        {
            // ACT
            var result = await CreateController().Post(CreateRequest(RandomString()));
            
            // ASSERT
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);
            Assert.Empty(response.Results);
        }


        [Fact]
        public async Task Post_ReturnsEmpty_WhenFunctionalRoleNamesIsEmpty()
        {
            // ACT
            var result = await CreateController().Post(CreateRequest());
            
            // ASSERT
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);
            Assert.Empty(response.Results);
        }

        [Fact]
        public async Task Post_HandlesDuplicateFunctionalRoleNames_WithoutDuplication()
        {
            // ARRAMGE
            var mapping = InsertTestMapping(RandomFunctionalRole(), RandomApplicationRole(), RandomDomain(RandomEntityType()), isAllEntityTypes: false);
            
            // ACT
            var result = await CreateController().Post(CreateRequest(mapping.FunctionalRole.Name, mapping.FunctionalRole.Name));
            
            // ASSERT
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            var singleResult = Assert.Single(response.Results);
            Assert.NotNull(singleResult.EntityType);
            var singleAppRole = Assert.Single(singleResult.ApplicationRoles);
            Assert.Equal(mapping.ApplicationRole.Name, singleAppRole.Name);
        }

        [Fact]
        public async Task Post_HandlesDuplicateMappings_WithoutDuplicateApplicationRoles()
        {
            // ARRANGE
            var entityType = RandomEntityType();
            var domain1 = RandomDomain(entityType);
            var domain2 = RandomDomain(entityType);

            var applicationRole = RandomApplicationRole();
            var mapping1 = InsertTestMapping(RandomFunctionalRole(), applicationRole, domain1, isAllEntityTypes: false);
            var mapping2 = InsertTestMapping(RandomFunctionalRole(), applicationRole, domain2, isAllEntityTypes: false);

            // ACT
            var result = await CreateController().Post(CreateRequest(mapping1.FunctionalRole.Name, mapping2.FunctionalRole.Name));
            
            // ASSERT
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);
            var singleResult = Assert.Single(response.Results);
            Assert.NotNull(singleResult.EntityType);
            Assert.Equal(entityType.Name, singleResult.EntityType.Name);
            var singleAppRole = Assert.Single(singleResult.ApplicationRoles);
            Assert.Equal(applicationRole.Name, singleAppRole.Name);
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


            var entity1Result = response.Results.FirstOrDefault(x=>x.EntityType?.Name == entity1.Name);
            Assert.NotNull(entity1Result);
            var entity1ResultSingleAppRole = Assert.Single(entity1Result.ApplicationRoles);
            Assert.Equal(mapping.ApplicationRole.Name, entity1ResultSingleAppRole.Name);

            var entity2Result = response.Results.FirstOrDefault(x => x.EntityType?.Name == entity2.Name);
            Assert.NotNull(entity2Result);
            var entity2ResultSingleAppRole = Assert.Single(entity2Result.ApplicationRoles);
            Assert.Equal(mapping.ApplicationRole.Name, entity2ResultSingleAppRole.Name);

        }

        [Fact]
        public async Task Post_CombinesBothMappings_WhenBothExist()
        {
            // ARRANGE
            var mapping1 = InsertTestMapping(RandomFunctionalRole(), RandomApplicationRole(), RandomDomain(RandomEntityType()), isAllEntityTypes:  false);
            var mapping2 = InsertTestMapping(RandomFunctionalRole(), RandomApplicationRole(), RandomDomain(RandomEntityType()), isAllEntityTypes:  false);

            // ACT
            var result = await CreateController().Post(CreateRequest(mapping1.FunctionalRole.Name, mapping2.FunctionalRole.Name));
            
            // ASSERT
            var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);

            Assert.Collection(response.Results,
                // from mapping 1
                r =>
                {
                    var singleAppRole = Assert.Single(r.ApplicationRoles);
                    Assert.Equal(mapping1.ApplicationRole.Name, singleAppRole.Name);
                    Assert.NotNull(r.EntityType);
                    Assert.Equal(r.EntityType.Name, mapping1.Domain!.EntityTypes[0].Name);
                },
                // from mapping 2;
                r =>
                {
                    var singleAppRole = Assert.Single(r.ApplicationRoles);
                    Assert.Equal(mapping2.ApplicationRole.Name, singleAppRole.Name);
                    Assert.NotNull(r.EntityType);
                    Assert.Equal(r.EntityType.Name, mapping2.Domain!.EntityTypes[0].Name);
                });
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

            // add functionalRole if it doesn't exist yet
            if(!_dbContext.FunctionalRoles.Any(x => x.Id == functionalRole.Id)){
                _dbContext.Add(functionalRole);
            }

            // add applicationRole if it doesn't exist yet
            if (!_dbContext.ApplicationRoles.Any(x => x.Id == applicationRole.Id)){
                _dbContext.Add(applicationRole);
            }

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
            _dbContext.Add(mapping);
            _dbContext.SaveChanges();
            
            return mapping;
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
