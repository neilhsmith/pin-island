using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using PinIsland.Api.Database;
using PinIsland.Api.Domain.Authorization;

namespace PinIsland.Api.Attributes;

public class HasPermissionAttribute : Attribute, IAuthorizationFilter
{
  private readonly string _permission;

  public HasPermissionAttribute(string permission)
  {
    _permission = permission;
  }

  public void OnAuthorization(AuthorizationFilterContext context)
  {
    var claim = context.HttpContext.User.Claims
      .FirstOrDefault(claim => claim.Type == "realm_access");
    if (claim == null)
    {
      context.Result = new ForbidResult();
      return;
    }

    var roles = JsonConvert.DeserializeObject<RealmAccessClaim>(claim.Value)?.Roles;
    if (roles == null)
    {
      context.Result = new ForbidResult();
      return;
    }

    if (roles.Contains(Role.SuperUser.Value))
    {
      return;
    }

    var dbContext = context.HttpContext
      .RequestServices
      .GetService(typeof(AppDbContext)) as AppDbContext;

    if (dbContext == null)
    {
      context.Result = new StatusCodeResult(500);
      return;
    }

    // all permissions granted to the user's roles
    var grantedPermissions = dbContext.RolePermissions
      .Where(rp => roles.Contains(rp.Role))
      .Select(rp => rp.Permission)
      .ToList();

    if (!grantedPermissions.Contains(_permission))
    {
      context.Result = new ForbidResult();
    }
  }
}

class RealmAccessClaim
{
  public ICollection<string>? Roles { get; set; }
}