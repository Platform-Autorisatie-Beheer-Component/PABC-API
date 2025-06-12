using PABC.Server.Features.GetApplicationRolesPerEntityType;
using PABC.Server.Test.TestConfig;

namespace PABC.Server.Test.Features.GetApplicationRolesPerEntityType
{
    public class GetApplicationRolesPerEntityTypeControllerTests(PostgresFixture postgresFixture) : IClassFixture<PostgresFixture>
    {
        [Fact]
        public async Task DummyTest()
        {
            var controller = new GetApplicationRolesPerEntityTypeController(postgresFixture.DbContext);
            var result = await controller.Post(new GetApplicationRolesRequest { FunctionalRoleNames = [] });
            Assert.NotNull(result);
        }
    }
}
