using PinIsland.Api.Domain.Roles;

namespace PinIsland.Api.Domain.Users;

public sealed class User : BaseEntity
{
  public string Identifier { get; set; } = default!;
  public string Username { get; set; } = default!;
  public string Email { get; set; } = default!;

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Website { get; set; }

  public ICollection<Role> Roles { get; set; } = new List<Role>();
}