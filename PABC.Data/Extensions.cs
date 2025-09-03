using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using EFCore.NamingConventions;

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
                .UseSnakeCaseNamingConvention();
            });

        return builder;
    }
}
