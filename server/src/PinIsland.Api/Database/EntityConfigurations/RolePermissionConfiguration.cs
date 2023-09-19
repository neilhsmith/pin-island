using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PinIsland.Api.Domain.Authorization;

namespace PinIsland.Api.Database.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
  public void Configure(EntityTypeBuilder<RolePermission> builder)
  {
    builder.Property(rp => rp.Role)
      .HasMaxLength(256);

    builder.Property(rp => rp.Permission)
      .HasMaxLength(256);
  }
}