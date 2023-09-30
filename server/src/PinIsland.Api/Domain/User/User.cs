
using PinIsland.Api.Domain.Groups;

namespace PinIsland.Api.Domain.User;

public class User : BaseEntity
{
  public string Identifier { get; set; } = null!;

  public string PreferredUsername { get; set; } = null!;

  public bool EmailVerified { get; set; } = default!;

  public string? Email { get; set; }

  public string? FirstName { get; set; }

  public string? LastName { get; set; }

  public string? Webpage { get; set; }

  public ICollection<Group>? Groups { get; set; }
}