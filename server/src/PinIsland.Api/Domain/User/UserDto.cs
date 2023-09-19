namespace PinIsland.Api.Domain.User;

public class UserDto
{
  public Guid Id { get; set; }
  public string Identifier { get; set; } = default!;
  public string PreferredUsername { get; set; } = default!;
  public bool EmailVerified { get; set; } = default!;
  public string? Email { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Webpage { get; set; }
}