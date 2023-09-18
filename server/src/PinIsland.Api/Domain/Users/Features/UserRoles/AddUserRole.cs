using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Extensions;

namespace PinIsland.Api.Domain.Users.Features.UserRoles;

public static class AddUserRole
{
  public sealed record Command(Guid UserId, AddUserRoleDto AddUserRoleDto) : IRequest;

  public sealed class Handler : IRequestHandler<Command>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
      var user = await _dbContext.Users.FirstAsync(user => user.Id == request.UserId);
      var role = await _dbContext.Roles.FirstAsync(role => role.NormalizedName == request.AddUserRoleDto.Name.Sanatize());

      user.Roles.Add(role);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
  }
}

public sealed class AddUserRoleDto
{
  public string Name { get; set; } = default!;
}