using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.MigrationService.Features.Prefill;

public interface IPrefillService
{
    Task Prefill(IReadOnlyList<PrefillApplication> applications, CancellationToken cancellationToken);
}

public class PrefillService(PabcDbContext dbContext, ILogger<PrefillService> logger) : IPrefillService
{
    public async Task Prefill(IReadOnlyList<PrefillApplication> applications, CancellationToken cancellationToken)
    {
        foreach (var config in applications)
        {
            var existing = await dbContext.Applications
                .FirstOrDefaultAsync(a => a.Name == config.Name, cancellationToken);

            if (existing != null)
            {
                logger.LogInformation("Prefill: applicatie '{Name}' bestaat al, overgeslagen", config.Name);
                continue;
            }

            var application = new Application
            {
                Id = Guid.NewGuid(),
                Name = config.Name
            };

            dbContext.Applications.Add(application);

            foreach (var roleName in config.Roles)
            {
                dbContext.ApplicationRoles.Add(new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    ApplicationId = application.Id
                });
            }

            logger.LogInformation(
                "Prefill: applicatie '{Name}' aangemaakt met {Count} applicatierol(len)",
                config.Name,
                config.Roles.Count);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
