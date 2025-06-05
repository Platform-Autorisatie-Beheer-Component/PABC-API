using Microsoft.EntityFrameworkCore;

namespace PABC.Data
{
    public class PabcDbContext(DbContextOptions options): DbContext(options)
    {
    }
}
