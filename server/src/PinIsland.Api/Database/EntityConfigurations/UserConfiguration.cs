using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PinIsland.Api.Domain.User;

namespace PinIsland.Api.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.Property(u => u.Identifier)
      .HasMaxLength(256);

    builder.Property(u => u.PreferredUsername)
      .HasMaxLength(64);

    builder.Property(u => u.Email)
      .HasMaxLength(320);

    builder.Property(u => u.FirstName)
      .HasMaxLength(128);

    builder.Property(u => u.LastName)
      .HasMaxLength(128);

    builder.Property(u => u.Webpage)
      .HasMaxLength(1028);
  }
}