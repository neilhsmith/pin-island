using System.Reflection;

namespace PinIsland.Api.Domain.Authorization;

public static class Permissions
{
  public const string CanReadRole = "role:read";
  public const string CanReadRoles = "roles:read";
  public const string CanCreateRole = "role:create";
  public const string CanUpdateRole = "role:update";
  public const string CanDeleteRole = "role:delete";

  public const string CanReadUser = "user:read";
  public const string CanReadUsers = "users:read";
  public const string CanCreateUser = "user:create";
  public const string CanUpdateUser = "user:update";
  public const string CanDeleteUser = "user:delete";

  public const string CanCreateRolePermission = "role-permission:create";
  public const string CanDeleteRolePermission = "role-permission:delete";

  public const string CanCreateUserRole = "user-role:create";
  public const string CanDeleteUserRole = "user-role:delete";

  public static IEnumerable<string> ToEnumerable()
  {
    return typeof(Permissions)
      .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
      .Where(fieldInfo => fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && fieldInfo.FieldType == typeof(string))
      .Select(fieldInfo => (string?)fieldInfo.GetRawConstantValue() ?? "")
      .Where(value => !string.IsNullOrEmpty(value))
      .ToList();
  }

  public static List<string> ToList()
  {
    return ToEnumerable().ToList();
  }
}