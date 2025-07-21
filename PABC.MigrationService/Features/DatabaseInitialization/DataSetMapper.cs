using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PABC.Data.Entities;

namespace PABC.MigrationService.Features.DatabaseInitialization
{
    public static class DataSetMapper
    {
        /// <summary>
        /// Maps the deserialized JSON dataSet into EF Core entity objects for persistence.
        /// Handles relationships between domains, roles, and mappings.
        /// </summary>
        public static IEnumerable<object> MapToEntities(DataSet dataSet)
        {
            var functionalRoles = dataSet.FunctionalRoles
                .Select(functionalRole => new FunctionalRole
                {
                    Id = functionalRole.Id,
                    Name = functionalRole.Name
                })
                .ToList();

            var appRoles = dataSet.ApplicationRoles
                .Select(appRole => new ApplicationRole
                {
                    Id = appRole.Id,
                    Application = appRole.Application,
                    Name = appRole.Name
                })
                .ToList();

            var entityTypes = dataSet.EntityTypes
                .Select(entityType => new EntityType
                {
                    Id = entityType.Id,
                    Name = entityType.Name,
                    EntityTypeId = entityType.EntityTypeId,
                    Type = entityType.Type,
                    Uri = entityType.Uri
                })
                .ToList();

            var domains = dataSet.Domains
                .Select(domain => new Domain
                {
                    Id = domain.Id,
                    Name = domain.Name,
                    Description = domain.Description,
                    EntityTypes = [.. entityTypes.Where(e => domain.EntityTypes.Contains(e.Id))]
                })
                .ToList();

            var mappings = dataSet.Mappings
                .Select(mapping => new Mapping
                {
                    Id = mapping.Id,
                    ApplicationRole = appRoles.Single(a => a.Id == mapping.ApplicationRole),
                    FunctionalRole = functionalRoles.Single(f => f.Id == mapping.FunctionalRole),
                    Domain = domains.Single(d => d.Id == mapping.Domain),
                })
                .ToList();

            return functionalRoles
                .Cast<object>()
                .Concat(appRoles)
                .Concat(entityTypes)
                .Concat(domains)
                .Concat(mappings);
        }
    }
}
