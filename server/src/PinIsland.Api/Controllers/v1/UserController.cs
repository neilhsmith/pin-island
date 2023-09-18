using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinIsland.Api.Domain.Users;
using PinIsland.Api.Domain.Users.Features;
using PinIsland.Api.Domain.Users.Features.UserRoles;

namespace PinIsland.Api.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
  private readonly ILogger<UserController> _logger;
  private readonly IMediator _mediator;

  public UserController(ILogger<UserController> logger, IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  [HttpGet("{id:guid}", Name = "GetUser")]
  [Authorize(Policy = "read_access")]
  public async Task<ActionResult<UserDto>> GetUser(Guid id)
  {
    var query = new GetUser.Query(id);
    var response = await _mediator.Send(query);
    return Ok(response);
  }

  [HttpGet(Name = "GetUsers")]
  [Authorize(Policy = "read_access")]
  public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] GetUserListDto getUserListDto)
  {
    var query = new GetUserList.Query(getUserListDto);
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

  [HttpPost(Name = "AddUser")]
  [Authorize(Policy = "write_access")]
  public async Task<ActionResult<UserDto>> AddUser([FromBody] AddUserDto addUserDto)
  {
    var command = new AddUser.Command(addUserDto);
    var response = await _mediator.Send(command);
    return CreatedAtRoute("GetUser", new { response.Id }, response);
  }

  [HttpPut(Name = "UpdateUser")]
  [Authorize(Policy = "write_access")]
  public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
  {
    var command = new UpdateUser.Command(id, updateUserDto);
    await _mediator.Send(command);
    return NoContent();
  }

  [HttpDelete(Name = "DeleteUser")]
  [Authorize(Policy = "write_access")]
  public async Task<ActionResult> DeleteUser(Guid id)
  {
    var command = new DeleteUser.Command(id);
    await _mediator.Send(command);
    return NoContent();
  }

  [HttpPost("{userId:guid}/roles", Name = "AddUserRole")]
  [Authorize(Policy = "write_access")]
  public async Task<ActionResult> AddUserRole([FromRoute] Guid userId, AddUserRoleDto addUserRoleDto)
  {
    var command = new AddUserRole.Command(userId, addUserRoleDto);
    await _mediator.Send(command);
    return NoContent();
  }

  [HttpDelete("{userId:guid}/roles/{roleId:guid}", Name = "DeleteUserRole")]
  [Authorize(Policy = "write_access")]
  public async Task<ActionResult> DeleteUserRole([FromRoute] Guid userId, [FromRoute] Guid roleId)
  {
    var command = new DeleteUserRole.Command(userId, roleId);
    await _mediator.Send(command);
    return NoContent();
  }
}