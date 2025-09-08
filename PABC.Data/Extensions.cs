using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace PABC.Data;

public static class Extensions
{
    public static IHostApplicationBuilder AddPabcDbContext(this IHostApplicationBuilder builder, string connectionName = "Pabc")
    {
        builder.AddNpgsqlDbContext<PabcDbContext>(
            connectionName: connectionName,
            configureDbContextOptions: options =>
            {

                options.UseNpgsql()
                .ReplaceService<IHistoryRepository, PascalCaseHistoryContext>()
                .UseSnakeCaseNamingConvention();
            });

        return builder;
    }

    /// <summary>
    /// workaround to keep the migrations working after using the snake case naming convention
    /// </summary>
    /// <param name="dependencies"></param>
#pragma warning disable EF1001 // Internal EF Core API usage.
    private class PascalCaseHistoryContext(HistoryRepositoryDependencies dependencies) : NpgsqlHistoryRepository(dependencies)
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
        protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);

            history.Property(h => h.MigrationId).HasColumnName("MigrationId");
            history.Property(h => h.ProductVersion).HasColumnName("ProductVersion");
        }
    }
}
