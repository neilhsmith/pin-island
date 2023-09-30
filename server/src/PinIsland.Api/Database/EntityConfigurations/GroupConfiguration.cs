using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PinIsland.Api.Domain.Groups;

namespace PinIsland.Api.Database.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
  public void Configure(EntityTypeBuilder<Group> builder)
  {
    builder.Property(g => g.Name)
      .HasMaxLength(64);

    builder.Property(g => g.Description)
      .HasMaxLength(256);
  }
}