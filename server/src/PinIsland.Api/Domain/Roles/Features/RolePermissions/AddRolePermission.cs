using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Domain.Authorization;

namespace PinIsland.Api.Domain.Roles.Features.RolePermissions;

public static class AddRolePermission
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
      if (Permissions.ToList().Any(permission => permission == request.Permission) == false)
      {
        // TODO: correct exception type
        throw new ArgumentException();
      }

      var role = await _dbContext.Roles.FirstAsync(role => role.Id == request.RoleId, cancellationToken);

      var rolePermission = new RolePermission
      {
        RoleId = role.Id,
        Permission = request.Permission
      };

      await _dbContext.RolePermissions.AddAsync(rolePermission, cancellationToken);
      await _dbContext.SaveChangesAsync();
    }
  }
}