using FluentValidation;
using MediatR;
using PinIsland.Api.Database;
using PinIsland.Api.Extensions;

namespace PinIsland.Api.Domain.Roles.Features;

public static class AddRole
{
  public sealed record Command(AddRoleDto AddRoleDto) : IRequest<RoleDto>;

  public sealed class Handler : IRequestHandler<Command, RoleDto>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<RoleDto> Handle(Command request, CancellationToken cancellationToken)
    {
      var validator = new AddRoleValidator();
      validator.ValidateAndThrow(request.AddRoleDto);

      var role = new Role
      {
        Name = request.AddRoleDto.Name,
        NormalizedName = request.AddRoleDto.Name.Sanatize()
      };

      await _dbContext.Roles.AddAsync(role, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);

      var roleDto = new RoleDto
      {
        Id = role.Id,
        Name = role.Name
      };

      return roleDto;
    }
  }

  private sealed class AddRoleValidator : AbstractValidator<AddRoleDto>
  {
    public AddRoleValidator()
    {
      RuleFor(createRole => createRole.Name)
        .NotEmpty()
        .MaximumLength(64);
    }
  }
}

public class AddRoleDto
{
  public string Name { get; set; } = default!;
}
