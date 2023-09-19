using System.Reflection;

namespace PinIsland.Api.Domain.Authorization;

public static class Permissions
{
  public const string CanReadPrivate = nameof(CanReadPrivate);
  public const string CanReadPrivateSuperUser = nameof(CanReadPrivateSuperUser);
  public const string CanReadPrivateAdmin = nameof(CanReadPrivateAdmin);
  public const string CanReadPrivateSuperAdmin = nameof(CanReadPrivateSuperAdmin);

  public const string CanReadRolePermission = nameof(CanReadRolePermission);
  public const string CanReadRolePermissions = nameof(CanReadRolePermissions);
  public const string CanCreateRolePermission = nameof(CanCreateRolePermission);
  public const string CanDeleteRolePermission = nameof(CanDeleteRolePermission);

  public static IEnumerable<string> ToEnumerable()
  {
    return typeof(Permissions)
      .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
      .Where(fieldInfo => fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && fieldInfo.FieldType == typeof(string))
      .Select(fieldInfo => (string?)fieldInfo.GetRawConstantValue() ?? "")
      .Where(value => !string.IsNullOrEmpty(value));
  }

  public static IList<string> ToList()
  {
    return ToEnumerable().ToList();
  }
}
