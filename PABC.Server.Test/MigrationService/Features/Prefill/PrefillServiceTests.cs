using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PABC.Data.Entities;
using PABC.MigrationService.Features.Prefill;
using PABC.Server.Test.TestConfig;

namespace PABC.Server.Test.MigrationService.Features.Prefill;

public class PrefillServiceTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private readonly Mock<ILogger<PrefillService>> _loggerMock = new();

    public async Task InitializeAsync()
    {
        var db = fixture.DbContext;
        db.ChangeTracker.Clear();
        await db.ApplicationRoles.ExecuteDeleteAsync();
        await db.Applications.ExecuteDeleteAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private PrefillService CreateService() => new(fixture.DbContext, _loggerMock.Object);

    [Fact]
    public async Task Prefill_CreatesApplicationAndRoles_WhenApplicationDoesNotExist()
    {
        // Arrange
        var applications = new List<PrefillApplication>
        {
            new() { Name = "testapp", Roles = ["role1", "role2", "role3"] }
        };

        // Act
        await CreateService().Prefill(applications, CancellationToken.None);

        // Assert
        fixture.DbContext.ChangeTracker.Clear();
        var app = await fixture.DbContext.Applications.FirstOrDefaultAsync(a => a.Name == "testapp");
        Assert.NotNull(app);

        var roles = await fixture.DbContext.ApplicationRoles
            .Where(r => r.ApplicationId == app.Id)
            .Select(r => r.Name)
            .ToListAsync();

        Assert.Equal(3, roles.Count);
        Assert.Contains("role1", roles);
        Assert.Contains("role2", roles);
        Assert.Contains("role3", roles);
    }

    [Fact]
    public async Task Prefill_SkipsApplication_WhenApplicationAlreadyExists()
    {
        // Arrange — create existing application
        fixture.DbContext.Applications.Add(new Application
        {
            Id = Guid.NewGuid(),
            Name = "existing-app"
        });
        await fixture.DbContext.SaveChangesAsync();

        var applications = new List<PrefillApplication>
        {
            new() { Name = "existing-app", Roles = ["should-not-be-created"] }
        };

        // Act
        await CreateService().Prefill(applications, CancellationToken.None);

        // Assert
        fixture.DbContext.ChangeTracker.Clear();
        var roles = await fixture.DbContext.ApplicationRoles
            .Where(r => r.Application.Name == "existing-app")
            .ToListAsync();

        Assert.Empty(roles);
    }

    [Fact]
    public async Task Prefill_HandlesMultipleApplications()
    {
        // Arrange
        fixture.DbContext.Applications.Add(new Application
        {
            Id = Guid.NewGuid(),
            Name = "app-exists"
        });
        await fixture.DbContext.SaveChangesAsync();

        var applications = new List<PrefillApplication>
        {
            new() { Name = "app-exists", Roles = ["skipped-role"] },
            new() { Name = "app-new", Roles = ["new-role"] }
        };

        // Act
        await CreateService().Prefill(applications, CancellationToken.None);

        // Assert
        fixture.DbContext.ChangeTracker.Clear();
        var newApp = await fixture.DbContext.Applications.FirstOrDefaultAsync(a => a.Name == "app-new");
        Assert.NotNull(newApp);

        var newRoles = await fixture.DbContext.ApplicationRoles
            .Where(r => r.ApplicationId == newApp.Id)
            .ToListAsync();
        Assert.Single(newRoles);
        Assert.Equal("new-role", newRoles[0].Name);

        var existingRoles = await fixture.DbContext.ApplicationRoles
            .Where(r => r.Application.Name == "app-exists")
            .ToListAsync();
        Assert.Empty(existingRoles);
    }

    [Fact]
    public async Task Prefill_DoesNothing_WhenListIsEmpty()
    {
        // Act
        await CreateService().Prefill([], CancellationToken.None);

        // Assert
        var count = await fixture.DbContext.Applications.CountAsync();
        Assert.Equal(0, count);
    }
}
