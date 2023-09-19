using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Domain.Authorization.Dtos;

namespace PinIsland.Api.Domain.Authorization.Features;

public static class DeleteRolePermission
{
  public sealed record Command(Guid Id)
    : IRequest;

  public sealed class Handler : IRequestHandler<Command>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
      var rolePermission = await _dbContext.RolePermissions
        .FirstAsync(rp => rp.Id == request.Id, cancellationToken);

      _dbContext.RolePermissions.Remove(rolePermission);
      await _dbContext.SaveChangesAsync();
    }
  }
}