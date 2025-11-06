using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Test.TestConfig
{
    public sealed class PostgresFixture : IAsyncLifetime
    {
        private readonly DistributedApplication _app;
        private readonly IResourceBuilder<PostgresServerResource> _postgres;
        private IServiceScope? _serviceScope;

        public PabcDbContext DbContext { get; private set; } = null!;

        public PostgresFixture()
        {
            var options = new DistributedApplicationOptions { AssemblyName = typeof(PostgresFixture).Assembly.FullName, DisableDashboard = true };
            var appBuilder = DistributedApplication.CreateBuilder(options);
            _postgres = appBuilder.AddPostgres("postgres");
            _postgres.AddDatabase("Pabc");
            _app = appBuilder.Build();
        }

        public async Task InitializeAsync()
        {
            await _app.StartAsync();
            await _app.ResourceNotifications.WaitForResourceHealthyAsync("Pabc");
            var _postgresConnectionString = await _postgres.Resource.GetConnectionStringAsync();
            var services = new ServiceCollection();
            services.AddDbContext<PabcDbContext>(opt =>
            opt.UseNpgsql(_postgresConnectionString)
            .UseSnakeCaseNamingConvention());
            _serviceScope = services.BuildServiceProvider().CreateScope();
            DbContext = _serviceScope.ServiceProvider.GetRequiredService<PabcDbContext>();
            await DbContext.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            _serviceScope?.Dispose();
            await _app.StopAsync();
            if (_app is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                _app.Dispose();
            }
        }

        public async Task<Application> CreateTestApplicationAsync()
        {
            var application = new Application
            {
                Id = Guid.NewGuid(),
                Name = $"TestApplication_{Guid.NewGuid()}"
            };

            DbContext.Add(application);
            await DbContext.SaveChangesAsync();
            
            return application;
        }
    }
}
