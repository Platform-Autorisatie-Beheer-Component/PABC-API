using System.Diagnostics;
using PABC.MigrationService.Features.DatabaseInitialization;

namespace PABC.MigrationService;

/// <summary>
/// A background worker that handles EF Core migrations and loads initial data from a JSON dataset.
/// If migrations fail, the process exits with a non-zero code to prevent application startup.
/// See https://learn.microsoft.com/en-us/dotnet/aspire/database/ef-core-migrations
/// </summary>
public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration, IDatasetParser datasetParser, ILogger<Worker> logger) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    // Used for distributed tracing and diagnostics
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    /// <summary>
    /// Entry point of the background worker.
    /// Runs the database migration and loads the dataset within a transaction.
    /// If any error occurs, logs the exception and halts the application startup.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);
        try
        {
            var dataSet = await GetDataSet(cancellationToken);
            await using var scope = serviceProvider.CreateAsyncScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
            await initializer.Initialize(dataSet, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            logger.LogCritical(ex, "Migrations failed");
            // Exit process with error code to block PABC.Server startup
            Environment.ExitCode = 1;
        }
        finally
        {
            // Gracefully shut down the host application once work is complete
            hostApplicationLifetime.StopApplication();
        }
    }

    private async Task<DataSet?> GetDataSet(CancellationToken cancellationToken)
    {
        var dataSetPath = configuration["JSON_DATASET_PATH"];
        if (string.IsNullOrWhiteSpace(dataSetPath)) return null;

        await using var file = File.OpenRead(dataSetPath);
        return await datasetParser.Parse(file, cancellationToken);
    }
}
