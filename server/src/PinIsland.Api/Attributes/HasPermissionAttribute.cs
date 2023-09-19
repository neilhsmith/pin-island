using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using PinIsland.Api.Database;

namespace PinIsland.Api.Attributes;

public class HasPermissionAttribute : ActionFilterAttribute
{
  private readonly ICollection<string> _permissions;

  public HasPermissionAttribute(string permissions)
  {
    _permissions = permissions.Split(",");
  }

  public override void OnActionExecuting(ActionExecutingContext filterContext)
  {
    // get the user's roles from the httpcontext's claims
    // get the role<->permission map from the db or, ideally, a cache
    // get the acceptable roles for the given permissions
    // throw if the user's roles are not in the list of acceptable roles

    var claim = filterContext.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "realm_access");
    if (claim == null)
      throw new Exception("todo: forbidden");

    var roles = JsonConvert.DeserializeObject<RealmAccessClaim>(claim.Value)?.Roles;
    if (roles == null)
      throw new Exception("todo: forbidden");


  }
}

class RealmAccessClaim
{
  public ICollection<string>? Roles { get; set; }
}