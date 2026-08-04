using Microsoft.AspNetCore.Mvc;
using Moq;
using PABC.Data;
using PABC.Data.Entities;
using PABC.Server.Features.EntityTypes.ImportZaaktypes;
using PABC.Server.Test.TestConfig;
using PABC.Server.ZgwZaakregister;

namespace PABC.Server.Test.Features.ImportZaaktypes
{
    public class ImportZaaktypesControllerTests(PostgresFixture fixture)
        : IClassFixture<PostgresFixture>, IAsyncLifetime
    {
        private readonly PabcDbContext _dbContext = fixture.DbContext;
        private readonly Mock<IZgwCatalogiClient> _zgwClientMock = new();

        public async Task InitializeAsync()
        {
            _dbContext.ChangeTracker.Clear();
            _dbContext.EntityTypes.RemoveRange(_dbContext.EntityTypes);
            await _dbContext.SaveChangesAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private ImportZaaktypesController CreateController()
        {
            _dbContext.ChangeTracker.Clear();
            return new ImportZaaktypesController(_dbContext, _zgwClientMock.Object);
        }

        [Fact]
        public async Task ImportZaaktypes_CreatesNewEntityTypes_WhenNoneExist()
        {
            // Arrange
            var omschrijvingen = new List<string> { "Bezwaar", "Melding openbare ruimte" };
            _zgwClientMock.Setup(c => c.GetZaaktypeOmschrijvingen(It.IsAny<CancellationToken>()))
                .ReturnsAsync(omschrijvingen);

            // Act
            var result = await CreateController().ImportZaaktypes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportZaaktypesResponse>(okResult.Value);

            Assert.Equal(2, response.Created.Count);
            Assert.Contains("Bezwaar", response.Created);
            Assert.Contains("Melding openbare ruimte", response.Created);
            Assert.Empty(response.Skipped);
            Assert.Empty(response.Stale);

            // Verify persisted in database
            _dbContext.ChangeTracker.Clear();
            var savedEntityTypes = _dbContext.EntityTypes.Where(e => e.Type == "ZAAKTYPE").ToList();
            Assert.Equal(2, savedEntityTypes.Count);
            Assert.All(savedEntityTypes, e =>
            {
                Assert.Equal("ZAAKTYPE", e.Type);
                Assert.Equal(e.EntityTypeId, e.Name);
                Assert.Null(e.Uri);
            });
        }

        [Fact]
        public async Task ImportZaaktypes_SkipsExisting_WhenZaaktypeAlreadyExists()
        {
            // Arrange
            _dbContext.EntityTypes.Add(new EntityType
            {
                Id = Guid.NewGuid(),
                Type = "ZAAKTYPE",
                EntityTypeId = "Bezwaar",
                Name = "Bezwaar"
            });
            await _dbContext.SaveChangesAsync();

            _zgwClientMock.Setup(c => c.GetZaaktypeOmschrijvingen(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "Bezwaar", "Melding openbare ruimte" });

            // Act
            var result = await CreateController().ImportZaaktypes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportZaaktypesResponse>(okResult.Value);

            Assert.Single(response.Created);
            Assert.Contains("Melding openbare ruimte", response.Created);
            Assert.Single(response.Skipped);
            Assert.Contains("Bezwaar", response.Skipped);
            Assert.Empty(response.Stale);
        }

        [Fact]
        public async Task ImportZaaktypes_DetectsStale_WhenZaaktypeNoLongerInZaakregister()
        {
            // Arrange
            _dbContext.EntityTypes.Add(new EntityType
            {
                Id = Guid.NewGuid(),
                Type = "ZAAKTYPE",
                EntityTypeId = "Vergunning",
                Name = "Vergunning"
            });
            await _dbContext.SaveChangesAsync();

            _zgwClientMock.Setup(c => c.GetZaaktypeOmschrijvingen(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "Bezwaar" });

            // Act
            var result = await CreateController().ImportZaaktypes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportZaaktypesResponse>(okResult.Value);

            Assert.Single(response.Created);
            Assert.Contains("Bezwaar", response.Created);
            Assert.Empty(response.Skipped);
            Assert.Single(response.Stale);
            Assert.Contains("Vergunning", response.Stale);
        }

        [Fact]
        public async Task ImportZaaktypes_ReturnsEmpty_WhenNoZaaktypesInZaakregister()
        {
            // Arrange
            _zgwClientMock.Setup(c => c.GetZaaktypeOmschrijvingen(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string>());

            // Act
            var result = await CreateController().ImportZaaktypes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportZaaktypesResponse>(okResult.Value);

            Assert.Empty(response.Created);
            Assert.Empty(response.Skipped);
            Assert.Empty(response.Stale);
        }

        [Fact]
        public async Task ImportZaaktypes_IgnoresNonZaaktypeEntityTypes_WhenComparing()
        {
            // Arrange — an entity type with a different type should not interfere
            _dbContext.EntityTypes.Add(new EntityType
            {
                Id = Guid.NewGuid(),
                Type = "INFORMATIEOBJECTTYPE",
                EntityTypeId = "Bezwaar",
                Name = "Bezwaar"
            });
            await _dbContext.SaveChangesAsync();

            _zgwClientMock.Setup(c => c.GetZaaktypeOmschrijvingen(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "Bezwaar" });

            // Act
            var result = await CreateController().ImportZaaktypes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ImportZaaktypesResponse>(okResult.Value);

            Assert.Single(response.Created);
            Assert.Contains("Bezwaar", response.Created);
            Assert.Empty(response.Skipped);
        }

        [Fact]
        public async Task ImportZaaktypes_Returns500_WhenZgwClientThrows()
        {
            // Arrange
            _zgwClientMock.Setup(c => c.GetZaaktypeOmschrijvingen(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Connection refused"));

            // Act
            var result = await CreateController().ImportZaaktypes();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var problem = Assert.IsType<ProblemDetails>(statusResult.Value);
            Assert.Contains("Connection refused", problem.Detail);
        }
    }
}
