using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Extensions;

namespace PinIsland.Api.Domain.Users.Features.UserRoles;

public static class DeleteUserRole
{
  public sealed record Command(Guid UserId, Guid RoleId) : IRequest;

  public sealed class Handler : IRequestHandler<Command>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
      var user = await _dbContext.Users.Include(user => user.Roles).FirstAsync(user => user.Id == request.UserId);
      var role = await _dbContext.Roles.FirstAsync(role => role.Id == request.RoleId);

      user.Roles.Remove(role);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
  }
}