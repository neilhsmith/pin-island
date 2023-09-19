namespace PinIsland.Api.Domain.Authorization;

public class RolePermission : BaseEntity
{
  public Role Role { get; set; } = null!;
  public string Permission { get; set; } = default!;
}