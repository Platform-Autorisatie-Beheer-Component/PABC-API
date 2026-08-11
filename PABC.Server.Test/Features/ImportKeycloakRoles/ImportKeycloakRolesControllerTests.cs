using Microsoft.AspNetCore.Mvc;
using Moq;
using PABC.Data;
using PABC.Data.Entities;
using PABC.Server.Features.FunctionalRoles.ImportKeycloakRoles;
using PABC.Server.Keycloak;
using PABC.Server.Test.TestConfig;

namespace PABC.Server.Test.Features.ImportKeycloakRoles
{
    public class ImportKeycloakRolesControllerTests(PostgresFixture fixture)
        : IClassFixture<PostgresFixture>, IAsyncLifetime
    {
        private readonly PabcDbContext _dbContext = fixture.DbContext;
        private readonly Mock<IKeycloakAdminClient> _keycloakClientMock = new();

        public async Task InitializeAsync()
        {
            _dbContext.ChangeTracker.Clear();
            _dbContext.FunctionalRoles.RemoveRange(_dbContext.FunctionalRoles);
            await _dbContext.SaveChangesAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private ImportKeycloakRolesController CreateController()
        {
            _dbContext.ChangeTracker.Clear();
            return new ImportKeycloakRolesController(_dbContext, _keycloakClientMock.Object);
        }

        [Fact]
        public async Task ImportKeycloakRoles_CreatesNewRoles_WhenNoneExist()
        {
            // Arrange
            var roles = new List<RoleRepresentation>
            {
                new() { Name = "Behandelaar" },
                new() { Name = "Recordbeheerder" }
            };
            _keycloakClientMock.Setup(c => c.GetRealmRoles(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            // Act
            var result = await CreateController().ImportKeycloakRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportKeycloakRolesResponse>(okResult.Value);

            Assert.Equal(2, response.Created.Count);
            Assert.Contains("Behandelaar", response.Created);
            Assert.Contains("Recordbeheerder", response.Created);
            Assert.Empty(response.Skipped);
            Assert.Empty(response.Stale);

            // Verify persisted in database
            _dbContext.ChangeTracker.Clear();
            var savedRoles = _dbContext.FunctionalRoles.ToList();
            Assert.Equal(2, savedRoles.Count);
        }

        [Fact]
        public async Task ImportKeycloakRoles_SkipsExisting_WhenRoleAlreadyExists()
        {
            // Arrange
            _dbContext.FunctionalRoles.Add(new FunctionalRole
            {
                Id = Guid.NewGuid(),
                Name = "Behandelaar"
            });
            await _dbContext.SaveChangesAsync();

            _keycloakClientMock.Setup(c => c.GetRealmRoles(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RoleRepresentation>
                {
                    new() { Name = "Behandelaar" },
                    new() { Name = "Recordbeheerder" }
                });

            // Act
            var result = await CreateController().ImportKeycloakRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportKeycloakRolesResponse>(okResult.Value);

            Assert.Single(response.Created);
            Assert.Contains("Recordbeheerder", response.Created);
            Assert.Single(response.Skipped);
            Assert.Contains("Behandelaar", response.Skipped);
            Assert.Empty(response.Stale);
        }

        [Fact]
        public async Task ImportKeycloakRoles_SkipsCaseInsensitiveDuplicate_WhenRoleExistsWithDifferentCasing()
        {
            // Arrange — "Behandelaar" exists in PABC, Keycloak returns "behandelaar" (different casing)
            _dbContext.FunctionalRoles.Add(new FunctionalRole
            {
                Id = Guid.NewGuid(),
                Name = "Behandelaar"
            });
            await _dbContext.SaveChangesAsync();

            _keycloakClientMock.Setup(c => c.GetRealmRoles(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RoleRepresentation>
                {
                    new() { Name = "behandelaar" },
                    new() { Name = "Recordbeheerder" }
                });

            // Act
            var result = await CreateController().ImportKeycloakRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportKeycloakRolesResponse>(okResult.Value);

            Assert.Single(response.Created);
            Assert.Contains("Recordbeheerder", response.Created);
            Assert.Single(response.Skipped);
            Assert.Contains("behandelaar", response.Skipped);
            Assert.Empty(response.Stale);
        }

        [Fact]
        public async Task ImportKeycloakRoles_DetectsStale_WhenRoleNoLongerInKeycloak()
        {
            // Arrange
            _dbContext.FunctionalRoles.Add(new FunctionalRole
            {
                Id = Guid.NewGuid(),
                Name = "Verwijderd"
            });
            await _dbContext.SaveChangesAsync();

            _keycloakClientMock.Setup(c => c.GetRealmRoles(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RoleRepresentation>
                {
                    new() { Name = "Behandelaar" }
                });

            // Act
            var result = await CreateController().ImportKeycloakRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportKeycloakRolesResponse>(okResult.Value);

            Assert.Single(response.Created);
            Assert.Contains("Behandelaar", response.Created);
            Assert.Empty(response.Skipped);
            Assert.Single(response.Stale);
            Assert.Contains("Verwijderd", response.Stale);
        }

        [Fact]
        public async Task ImportKeycloakRoles_ReturnsEmpty_WhenNoRolesInKeycloak()
        {
            // Arrange
            _keycloakClientMock.Setup(c => c.GetRealmRoles(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RoleRepresentation>());

            // Act
            var result = await CreateController().ImportKeycloakRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportKeycloakRolesResponse>(okResult.Value);

            Assert.Empty(response.Created);
            Assert.Empty(response.Skipped);
            Assert.Empty(response.Stale);
        }

        [Fact]
        public async Task ImportKeycloakRoles_Returns500_WhenKeycloakClientThrows()
        {
            // Arrange
            _keycloakClientMock.Setup(c => c.GetRealmRoles(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Connection refused"));

            // Act
            var result = await CreateController().ImportKeycloakRoles();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var problem = Assert.IsType<ProblemDetails>(statusResult.Value);
            Assert.Contains("Connection refused", problem.Detail);
        }
    }
}
