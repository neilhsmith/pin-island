using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinIsland.Api.Attributes;
using PinIsland.Api.Domain.Authorization;

namespace PinIsland.Api.Controllers.v1;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
  private readonly ILogger<TestController> _logger;

  public TestController(ILogger<TestController> logger)
  {
    _logger = logger;
  }

  [HttpGet]
  [Route("public")]
  public ActionResult<TestDto> GetPublic()
  {
    return Ok(new TestDto
    {
      Result = "Success from the public test",
      Token = Request.Headers["Authorization"],
      Claims = User.Claims.ToList()
    });
  }

  [HttpGet]
  [Route("private")]
  [Authorize(Scopes.ReadAccess)]
  [HasPermission(Permissions.CanReadPrivate)]
  public ActionResult<TestDto> GetPrivate()
  {
    var user = Request.HttpContext.User;
    var identity = user.Identity;
    var claims = user.Claims.ToList();
    return Ok(new TestDto
    {
      Result = "Success from the private test",
      Token = Request.Headers["Authorization"],
      Claims = User.Claims.ToList()
    });
  }
}

public class TestDto
{
  public string? Result { get; set; }
  public string? Token { get; set; }
  public List<Claim>? Claims { get; set; }
}