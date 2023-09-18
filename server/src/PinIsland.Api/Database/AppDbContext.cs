using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database.Converters;
using PinIsland.Api.Domain;
using PinIsland.Api.Domain.Roles;
using PinIsland.Api.Extensions.Application;

namespace PinIsland.Api.Database;

public sealed class AppDbContext : DbContext
{
  public DbSet<Role> Roles { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.FilterSoftDeletedRecords();
  }

  protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
  {
    base.ConfigureConventions(configurationBuilder);

    configurationBuilder.Properties<DateOnly>()
      .HaveConversion<DateOnlyConverter>()
      .HaveColumnType("date");
  }

  public override int SaveChanges()
  {
    UpdateAuditFields();
    return base.SaveChanges();
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
  {
    UpdateAuditFields();
    return await base.SaveChangesAsync(cancellationToken);
  }

  private void UpdateAuditFields()
  {
    var now = DateTime.UtcNow;
    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
    {
      switch (entry.State)
      {
        case EntityState.Added:
          entry.Entity.UpdateCreationProperties(now);
          entry.Entity.UpdateModifiedProperties(now);
          break;
        case EntityState.Modified:
          entry.Entity.UpdateModifiedProperties(now);
          break;
        case EntityState.Deleted:
          entry.State = EntityState.Modified;
          entry.Entity.UpdateModifiedProperties(now);
          entry.Entity.UpdateIsDeleted(true);
          break;
      }
    }
  }
}