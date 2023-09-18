namespace PinIsland.Api.Domain.Roles;

public sealed class Role : BaseEntity
{
  public string Name { get; set; } = default!;
  public string NormalizedName { get; set; } = default!;
}