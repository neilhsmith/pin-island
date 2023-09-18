using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Extensions;

namespace PinIsland.Api.Domain.Roles.Features;

public static class UpdateRole
{
  public sealed record Command(Guid Id, UpdateRoleDto UpdateRoleDto) : IRequest;

  public sealed class Handler : IRequestHandler<Command>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
      var validator = new UpdateRoleValidator();
      validator.ValidateAndThrow(request.UpdateRoleDto);

      var role = await _dbContext.Roles.FirstAsync(role => role.Id == request.Id);

      role.Name = request.UpdateRoleDto.Name;
      role.NormalizedName = request.UpdateRoleDto.Name.Sanatize();

      _dbContext.Roles.Update(role);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
  }

  private sealed class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
  {
    public UpdateRoleValidator()
    {
      RuleFor(createRole => createRole.Name)
        .NotEmpty()
        .MaximumLength(64);
    }
  }
}

public class UpdateRoleDto
{
  public string Name { get; set; } = default!;
}
