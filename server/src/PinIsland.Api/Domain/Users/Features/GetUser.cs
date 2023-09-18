using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;

namespace PinIsland.Api.Domain.Users.Features;

public static class GetUser
{
  public sealed record Query(Guid Id) : IRequest<UserDto>;

  public sealed class Handler : IRequestHandler<Query, UserDto>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
    {
      var user = await _dbContext.Users.FirstAsync(User => User.Id == request.Id);

      var userDto = new UserDto
      {
        Id = user.Id,
        Identifier = user.Identifier,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Website = user.LastName
      };

      return userDto;
    }
  }
}