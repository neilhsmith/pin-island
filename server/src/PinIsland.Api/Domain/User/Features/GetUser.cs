using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;

namespace PinIsland.Api.Domain.User.Features;

public static class GetUser
{
  public sealed record Query(Guid Id)
    : IRequest<UserDto>;

  public sealed class Handler : IRequestHandler<Query, UserDto>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
    {
      var user = await _dbContext.Users.FirstAsync(
        user => user.Id == request.Id, cancellationToken);

      return new UserDto
      {
        Id = user.Id,
        Identifier = user.Identifier,
        PreferredUsername = user.PreferredUsername,
        EmailVerified = user.EmailVerified,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Webpage = user.Webpage,
      };
    }
  }
}