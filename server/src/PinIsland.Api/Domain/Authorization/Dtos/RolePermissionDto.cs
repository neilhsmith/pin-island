namespace PinIsland.Api.Domain.Authorization.Dtos;

public class RolePermissionDto
{
  public Guid Id { get; set; }
  public RoleDto Role { get; set; } = null!;
  public string Permission { get; set; } = default!;
}