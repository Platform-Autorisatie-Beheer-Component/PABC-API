using Microsoft.EntityFrameworkCore;
using PABC.Data.Entities;

namespace PABC.Data;

public class PabcDbContext(DbContextOptions options) : DbContext(options)
{
    public const int MaxLengthForIndexProperties = 256;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationRole>(r =>
        {
            r.Property(x => x.Application).HasMaxLength(MaxLengthForIndexProperties);
            r.Property(x => x.Name).HasMaxLength(MaxLengthForIndexProperties);
            r.HasIndex(x => new { x.Application, x.Name }).IsUnique();
        });

        modelBuilder.Entity<Domain>(d =>
        {
            d.Property(x => x.Name).HasMaxLength(MaxLengthForIndexProperties);
            d.HasIndex(x => new { x.Name }).IsUnique();
            d.HasMany(x => x.EntityTypes).WithMany();
        });

        modelBuilder.Entity<EntityType>(e =>
        {
            e.Property(x => x.EntityTypeId).HasMaxLength(MaxLengthForIndexProperties);
            e.Property(x => x.Type).HasMaxLength(MaxLengthForIndexProperties);
            e.HasIndex(x => new { x.Type, x.EntityTypeId }).IsUnique();
        });

        modelBuilder.Entity<FunctionalRole>(r =>
        {
            r.Property(x => x.Name).HasMaxLength(MaxLengthForIndexProperties);
            r.HasIndex(x => new { x.Name }).IsUnique();
        });

        modelBuilder.Entity<Mapping>(m =>
        {
            m.HasIndex(x => new { x.ApplicationRoleId, x.DomainId, x.FunctionalRoleId }).IsUnique();
            var domainIdColumn = m.Metadata.GetProperty(nameof(Mapping.DomainId)).GetColumnName();
            var isAllEntityTypesColumn = m.Metadata.GetProperty(nameof(Mapping.IsAllEntityTypes)).GetColumnName();
            m.ToTable(t => t.HasCheckConstraint(
                $"CK_Mapping_{domainIdColumn}_{isAllEntityTypesColumn}",
                $"(\"{isAllEntityTypesColumn}\" = true AND \"{domainIdColumn}\" IS NULL) OR (\"{isAllEntityTypesColumn}\" = false AND \"{domainIdColumn}\" IS NOT NULL)"));

            m.HasOne(x => x.Domain)
             .WithMany()
             .HasForeignKey(x => x.DomainId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public required DbSet<ApplicationRole> ApplicationRoles { get; set; }
    public required DbSet<Domain> Domains { get; set; }
    public required DbSet<EntityType> EntityTypes { get; set; }
    public required DbSet<FunctionalRole> FunctionalRoles { get; set; }
    public required DbSet<Mapping> Mappings { get; set; }
}
