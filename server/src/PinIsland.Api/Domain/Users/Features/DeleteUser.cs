using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;

namespace PinIsland.Api.Domain.Users.Features;

public static class DeleteUser
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
      var user = await _dbContext.Users.FirstAsync(user => user.Id == request.Id, cancellationToken);

      _dbContext.Users.Remove(user);
      await _dbContext.SaveChangesAsync();
    }
  }
}
