using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinIsland.Api.Domain;
using PinIsland.Api.Domain.Authorization;
using PinIsland.Api.Domain.Authorization.Dtos;
using PinIsland.Api.Domain.Authorization.Features;

namespace PinIsland.Api.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/role-permissions")]
public class RolePermissionsController : ControllerBase
{
  private readonly ILogger<RolePermissionsController> _logger;
  private readonly IMediator _mediator;

  public RolePermissionsController(ILogger<RolePermissionsController> logger, IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  [Authorize(Policy = Scopes.ReadAccess)]
  [HttpGet("{id:guid}", Name = "GetRolePermission")]
  public async Task<ActionResult<RolePermissionDto>> GetRolePermission(Guid id)
  {
    var query = new GetRolePermission.Query(id);
    var response = await _mediator.Send(query);
    return Ok(response);
  }

  [Authorize(Policy = Scopes.ReadAccess)]
  [HttpGet(Name = "GetRolePermissionList")]
  public async Task<ActionResult<PagedList<RolePermissionDto>>> GetRolePermissionList([FromQuery] RolePermissionParametersDto dto)
  {
    var query = new GetRolePermissionList.Query(dto);
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

    Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
    return Ok(response);
  }

  [Authorize(Policy = Scopes.WriteAccess)]
  [HttpPost(Name = "CreateRolePermission")]
  public async Task<ActionResult<RolePermissionDto>> CreateRolePermission(CreateRolePermissionDto dto)
  {
    var command = new AddRolePermission.Command(dto);
    var response = await _mediator.Send(command);
    return CreatedAtRoute(
      "GetRolePermission",
      new { response.Id },
      response
    );
  }

  [Authorize(Policy = Scopes.WriteAccess)]
  [HttpDelete("{id:guid}", Name = "DeleteRolePermission")]
  public async Task<ActionResult> DeleteRolePermission(Guid id)
  {
    var command = new DeleteRolePermission.Command(id);
    await _mediator.Send(command);
    return NoContent();
  }
}