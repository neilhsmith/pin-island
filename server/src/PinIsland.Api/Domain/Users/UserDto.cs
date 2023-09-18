namespace PinIsland.Api.Domain.Users;

public sealed record UserDto
{
  public Guid Id { get; set; } = default!;
  public string Identifier { get; set; } = default!;
  public string Username { get; set; } = default!;
  public string Email { get; set; } = default!;

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Website { get; set; }
}