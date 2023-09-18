using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;

namespace PinIsland.Api.Domain.Roles.Features;

public static class GetRole
{
  public sealed record Query(Guid Id) : IRequest<RoleDto>;

  public sealed class Handler : IRequestHandler<Query, RoleDto>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<RoleDto> Handle(Query request, CancellationToken cancellationToken)
    {
      var role = await _dbContext.Roles.FirstAsync(role => role.Id == request.Id);

      var roleDto = new RoleDto
      {
        Id = role.Id,
        Name = role.Name
      };

      return roleDto;
    }
  }
}