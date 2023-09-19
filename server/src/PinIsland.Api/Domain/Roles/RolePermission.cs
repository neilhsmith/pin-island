namespace PinIsland.Api.Domain.Roles;

public sealed class RolePermission : BaseEntity
{
  public Guid RoleId { get; set; }
  public Role Role { get; set; } = null!;

  public string Permission { get; set; } = default!;
}