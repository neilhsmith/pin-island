using Ardalis.SmartEnum;

namespace PinIsland.Api.Domain.Authorization;

public sealed class Role : SmartEnum<Role, string>
{
  public static readonly Role User = new Role(nameof(User), "pin-island_user");
  public static readonly Role SuperUser = new Role(nameof(SuperUser), "pin-island_super-user");
  public static readonly Role Admin = new Role(nameof(Admin), "pin-island_admin");
  public static readonly Role SuperAdmin = new Role(nameof(SuperAdmin), "pin-island_super-admin");

  public Role(string name, string value)
    : base(name, value)
  { }
}