using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinIsland.Api.Domain;
using PinIsland.Api.Domain.Authorization;
using PinIsland.Api.Domain.User;
using PinIsland.Api.Domain.User.Features;
using PinIsland.Api.Domain.Users.Features;

namespace PinIsland.Api.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
  private readonly ILogger<UsersController> _logger;
  private readonly IMediator _mediator;

  public UsersController(ILogger<UsersController> logger, IMediator mediator)
  {
    _logger = logger;
    _mediator = mediator;
  }

  [Authorize(Policy = Scopes.ReadAccess)]
  [HttpGet("{id:guid}", Name = "GetUser")]
  public async Task<ActionResult<UserDto>> GetUser(Guid id)
  {
    var query = new GetUser.Query(id);
    var response = await _mediator.Send(query);
    return Ok(response);
  }

  [Authorize(Policy = Scopes.ReadAccess)]
  [HttpGet(Name = "GetUserList")]
  public async Task<ActionResult<PagedList<UserDto>>> GetUserList([FromQuery] UserParametersDto dto)
  {
    var query = new GetUserList.Query(dto);
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
  [HttpPost(Name = "CreateUser")]
  public async Task<ActionResult<UserDto>> CreateUser(AddUserDto dto)
  {
    var command = new AddUser.Command(dto);
    var response = await _mediator.Send(command);
    return CreatedAtRoute(
      "GetUser",
      new { response.Id },
      response
    );
  }

  [Authorize(Policy = Scopes.WriteAccess)]
  [HttpPut("{id:guid}", Name = "UpdateUser")]
  public async Task<ActionResult<UserDto>> UpdateUser(Guid id, UpdateUserDto dto)
  {
    var command = new UpdateUser.Command(id, dto);
    var response = await _mediator.Send(command);
    return Ok(response);
  }

  [Authorize(Policy = Scopes.WriteAccess)]
  [HttpDelete("{id:guid}", Name = "DeleteUser")]
  public async Task<ActionResult> DeleteUser(Guid id)
  {
    var command = new DeleteUser.Command(id);
    await _mediator.Send(command);
    return NoContent();
  }
}