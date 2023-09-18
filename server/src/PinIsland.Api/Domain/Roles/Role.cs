using PinIsland.Api.Domain.Users;

namespace PinIsland.Api.Domain.Roles;

public sealed class Role : BaseEntity
{
  public string Name { get; set; } = default!;
  public string NormalizedName { get; set; } = default!;

  public ICollection<User> Users { get; set; } = new List<User>();
}