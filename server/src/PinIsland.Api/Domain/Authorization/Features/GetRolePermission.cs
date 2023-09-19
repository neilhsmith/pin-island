using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Domain.Authorization.Dtos;

namespace PinIsland.Api.Domain.Authorization.Features;

public static class GetRolePermission
{
  public sealed record Query(Guid Id)
    : IRequest<RolePermissionDto>;

  public sealed class Handler : IRequestHandler<Query, RolePermissionDto>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<RolePermissionDto> Handle(Query request, CancellationToken cancellationToken)
    {
      var rolePermission = await _dbContext.RolePermissions
        .FirstAsync(rp => rp.Id == request.Id);

      return new RolePermissionDto
      {
        Id = rolePermission.Id,
        Permission = rolePermission.Permission,
        Role = new RoleDto
        {
          Name = rolePermission.Role.Name,
          Value = rolePermission.Role.Value
        },
      };
    }
  }
}