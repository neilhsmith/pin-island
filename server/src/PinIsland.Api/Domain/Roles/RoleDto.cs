namespace PinIsland.Api.Domain.Roles;

public sealed record RoleDto
{
  public Guid Id { get; set; } = default!;
  public string Name { get; set; } = default!;
}