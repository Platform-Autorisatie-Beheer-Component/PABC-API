using Microsoft.EntityFrameworkCore;

namespace PABC.Server.Data
{
    public class PabcDbContext(DbContextOptions<PabcDbContext> options): DbContext(options)
    {
    }
}
