using Microsoft.AspNetCore.Mvc;
using PABC.Data;
using PABC.Data.Entities;
using PABC.Server.Features.GetApplicationRolesPerEntityType;
using PABC.Server.Test.TestConfig;

namespace PABC.Server.Test.Features.GetApplicationRolesPerEntityType;

public class GetApplicationRolesPerEntityTypeControllerTests(PostgresFixture fixture)
    : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private static readonly string ValidFunctionalRole = "FunctionalRoleA";
    private static readonly string InvalidFunctionalRole = "NonExistingRole";
    private static readonly string ApplicationRoleName = "AppRoleA";
    private static readonly string ApplicationName = "AppA";
    private static readonly string EntityTypeName = "EntityTypeA";
    private static readonly string EntityTypeType = "TypeA";
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
            { Id = Guid.NewGuid(), Name = ApplicationRoleName, Application = ApplicationName };
        var entityType = new EntityType
        {
            Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Name = EntityTypeName, Type = EntityTypeType
        };
        var domain1 = new Domain
            { Id = Guid.NewGuid(), EntityTypes = [entityType], Description = string.Empty, Name = "Domain 1" };
        var domain2 = new Domain
            { Id = Guid.NewGuid(), EntityTypes = [entityType], Description = string.Empty, Name = "Domain 2" };

        var mappings = new List<Mapping>
        {
            new()
            {
                Id = Guid.NewGuid(), FunctionalRole = functionalRole, ApplicationRole = applicationRole,
                Domain = domain1
            },
            new()
            {
                Id = Guid.NewGuid(), FunctionalRole = functionalRole, ApplicationRole = applicationRole,
                Domain = domain2
            } // will result in duplicate result in the query if we don't handle it explicitly
        };

        _dbContext.AddRange(functionalRole, applicationRole, entityType, domain1, domain2);
        _dbContext.AddRange(mappings);
        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Post_ReturnsExpectedData_ForValidFunctionalRole()
    {
        var result = await CreateController().Post(CreateRequest(ValidFunctionalRole));
        var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);
        var singleResult = Assert.Single(response.Results);
        var role = Assert.Single(singleResult.ApplicationRoles);
        Assert.Equal(ApplicationRoleName, role.Name);
        Assert.Equal(ApplicationName, role.Application);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenFunctionalRoleDoesNotExist()
    {
        var result = await CreateController().Post(CreateRequest(InvalidFunctionalRole));
        Assert.IsType<BadRequestObjectResult>(result.Result);
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
        var results = Assert.Single(response.Results);
        Assert.Single(results.ApplicationRoles);
    }

    [Fact]
    public async Task Post_HandlesDuplicateMappings_WithoutDuplicateApplicationRoles()
    {
        var result = await CreateController().Post(CreateRequest(ValidFunctionalRole));
        var response = Assert.IsType<GetApplicationRolesResponse>(result.Value);
        var results = Assert.Single(response.Results);
        Assert.Single(results.ApplicationRoles);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenMixedValidAndInvalid()
    {
        var result = await CreateController().Post(CreateRequest(ValidFunctionalRole, InvalidFunctionalRole));
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
