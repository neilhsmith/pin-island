using System.Text.Json;
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

  [HttpGet(Name = "GetRoles")]
  [Authorize(Policy = "read_access")]
  public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles([FromQuery] GetRoleListDto getRoleListDto)
  {
    var query = new GetRoleList.Query(getRoleListDto);
    var response = await _mediator.Send(query);

    var paginationMetadata = new
    {
      totalCount = response.TotalCount,
      pageSize = response.PageSize,
      currentPageSize = response.CurrentPageSize,
      currentStartIndex = response.CurrentStartIndex,
      currentEndIndex = response.CurrentEndIndex,
      pageNumber = response.PageNumber,
      totalPages = response.TotalPages,
      hasPrevious = response.HasPrevious,
      hasNext = response.HasNext
    };

    // TODO: create an extension for this
    Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

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

  [HttpPut(Name = "UpdateRole")]
  [Authorize(Policy = "write_access")]
  public async Task<ActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleDto updateRoleDto)
  {
    var command = new UpdateRole.Command(id, updateRoleDto);
    await _mediator.Send(command);
    return NoContent();
  }

  [HttpDelete(Name = "DeleteRole")]
  [Authorize(Policy = "write_access")]
  public async Task<ActionResult> DeleteRole(Guid id)
  {
    var command = new DeleteRole.Command(id);
    await _mediator.Send(command);
    return NoContent();
  }
}