using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Domain.Authorization;

namespace PinIsland.Api.Domain.Roles.Features.RolePermissions;

public static class DeleteRolePermission
{
  public sealed record Command(Guid RoleId, string Permission) : IRequest;

  public sealed class Handler : IRequestHandler<Command>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
      var rolePermission = await _dbContext.RolePermissions.FirstAsync(
        rolePermission => rolePermission.RoleId == request.RoleId && rolePermission.Permission == request.Permission, cancellationToken);

      _dbContext.RolePermissions.Remove(rolePermission);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
  }
}