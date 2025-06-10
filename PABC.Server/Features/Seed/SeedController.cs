using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;
using PABC.Server.Auth;
using PABC.Server.Features.GetApplicationRolesPerEntityType;
using System.Net.Mime;

namespace PABC.Server.Features.Seed
{


    [ApiController]
    [Route("/api/v1/seed")]
    [Authorize(Policy = ApiKeyAuthentication.Policy)]
    public class SeedController(PabcDbContext db) : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
        public async Task<ActionResult<GetApplicationRolesResponse>> GetAsync()
        {
            var seedResult = await Seed();
            return Ok(new { seedDataStored = seedResult });

        }



        private async Task<bool> Seed()
        {
            if (await db.EntityTypes.AnyAsync() ||
              await db.Domains.AnyAsync() ||
              await db.ApplicationRoles.AnyAsync() ||
              await db.FunctionalRoles.AnyAsync() ||
              await db.Mappings.AnyAsync()) return false;

            var appRole1 = new ApplicationRole { Id = Guid.NewGuid(), Application = "MyApplication1", Name = "MyAppRole1" };
            var appRole2 = new ApplicationRole { Id = Guid.NewGuid(), Application = "MyApplication2", Name = "MyAppRole2" };

            var funcRole1 = new FunctionalRole { Id = Guid.NewGuid(), Name = "MyFuncRole1" };
            var funcRole2 = new FunctionalRole { Id = Guid.NewGuid(), Name = "MyFuncRole2" };

            var entityType1 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = "zaaktype", Name = "My Zaaktype 1" };
            var entityType2 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = "zaaktype", Name = "My Zaaktype 2" };
            var entityType3 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = "zaaktype", Name = "My Zaaktype 3" };
            var entityType4 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = "zaaktype", Name = "My Zaaktype 4" };
            var entityType5 = new EntityType { Id = Guid.NewGuid(), EntityTypeId = Guid.NewGuid().ToString(), Type = "zaaktype", Name = "My Zaaktype 5" };

            var domain1 = new Domain { Id = Guid.NewGuid(), Description = "This is my first domain", Name = "MyDomain1", EntityTypes = [entityType1, entityType2, entityType3] };
            var domain2 = new Domain { Id = Guid.NewGuid(), Description = "This is my second domain", Name = "MyDomain2", EntityTypes = [entityType3, entityType4, entityType5] };

            var mapping1 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRole1, Domain = domain1, FunctionalRole = funcRole1 };
            var mapping2 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRole1, Domain = domain2, FunctionalRole = funcRole2 };
            var mapping3 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRole2, Domain = domain1, FunctionalRole = funcRole2 };
            var mapping4 = new Mapping { Id = Guid.NewGuid(), ApplicationRole = appRole2, Domain = domain2, FunctionalRole = funcRole1 };

            await db.AddRangeAsync(mapping1, mapping2, mapping3, mapping4);
            var result = await db.SaveChangesAsync();
            return result > 0;
        }

    }

}

