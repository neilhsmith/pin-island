namespace PinIsland.Api.Domain.Groups;

public class Group : BaseEntity
{
  public string Name { get; set; } = null!;

  public string? Description { get; set; }

  public bool IsDefault { get; set; }

  public ICollection<User.User>? Users { get; set; }
}