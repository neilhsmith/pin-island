using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PinIsland.Api.Domain.Roles;

namespace PinIsland.Api.Database.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> builder)
  {
    builder.HasIndex(x => x.NormalizedName)
      .IsUnique();
  }
}