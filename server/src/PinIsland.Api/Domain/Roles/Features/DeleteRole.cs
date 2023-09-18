using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;

namespace PinIsland.Api.Domain.Roles.Features;

public static class DeleteRole
{
  public sealed record Command(Guid Id) : IRequest;

  public sealed class Handler : IRequestHandler<Command>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
      var role = await _dbContext.Roles.FirstAsync(role => role.Id == request.Id, cancellationToken);

      _dbContext.Roles.Remove(role);
      await _dbContext.SaveChangesAsync();
    }
  }
}
