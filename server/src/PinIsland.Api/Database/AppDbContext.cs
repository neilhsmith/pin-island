using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database.Configurations;
using PinIsland.Api.Domain;
using PinIsland.Api.Domain.Authorization;
using PinIsland.Api.Domain.User;
using PinIsland.Api.Extensions.Application;
using SmartEnum.EFCore;

namespace PinIsland.Api.Database;

public sealed class AppDbContext : DbContext
{
  public DbSet<RolePermission> RolePermissions { get; set; }
  public DbSet<User> Users { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
  { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.FilterSoftDeletedRecords();

    modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
    modelBuilder.ApplyConfiguration(new UserConfiguration());
  }

  protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
  {
    configurationBuilder.ConfigureSmartEnum();
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