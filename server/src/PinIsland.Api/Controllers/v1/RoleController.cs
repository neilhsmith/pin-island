using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinIsland.Api.Domain.Roles;
using PinIsland.Api.Domain.Roles.Features;

namespace PinIsland.Api.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/role")]
public class RoleController : ControllerBase
{
  private readonly ILogger<RoleController> _logger;
  private readonly IMediator _mediator;

  public RoleController(ILogger<RoleController> logger, IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  [HttpGet("{id:guid}", Name = "GetRole")]
  [Authorize(Policy = "read_access")]
  public async Task<ActionResult<RoleDto>> GetRole(Guid id)
  {
    var query = new GetRole.Query(id);
    var response = await _mediator.Send(query);
    return Ok(response);
  }

  [HttpPost(Name = "AddRole")]
  [Authorize(Policy = "write_access")]
  public async Task<ActionResult<RoleDto>> AddRole([FromBody] AddRoleDto addRoleDto)
  {
    var command = new AddRole.Command(addRoleDto);
    var response = await _mediator.Send(command);
    return CreatedAtRoute("GetRole", new { response.Id }, response);
  }
}