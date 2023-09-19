using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Domain.Authorization.Dtos;

namespace PinIsland.Api.Domain.Authorization.Features;

public static class AddRolePermission
{
  public sealed record Command(CreateRolePermissionDto CreateRolePermissionDto)
    : IRequest<RolePermissionDto>;

  public sealed class Handler : IRequestHandler<Command, RolePermissionDto>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<RolePermissionDto> Handle(Command request, CancellationToken cancellationToken)
    {
      var validator = new CreateRolePermissionDtoValidator();
      validator.ValidateAndThrow(request.CreateRolePermissionDto);

      var existing = await _dbContext.RolePermissions
        .FirstOrDefaultAsync(rp =>
          rp.Role == request.CreateRolePermissionDto.Role && rp.Permission == request.CreateRolePermissionDto.Permission,
          cancellationToken
        );

      if (existing is not null)
      {
        // TODO: throw
      }

      var newRolePermission = new RolePermission
      {
        Role = Role.FromValue(request.CreateRolePermissionDto.Role),
        Permission = request.CreateRolePermissionDto.Permission!
      };

      await _dbContext.RolePermissions.AddAsync(newRolePermission, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);

      return new RolePermissionDto
      {
        Id = newRolePermission.Id,
        Permission = newRolePermission.Permission,
        Role = new RoleDto
        {
          Name = newRolePermission.Role.Name,
          Value = newRolePermission.Role.Value
        },
      };
    }
  }
}

public class CreateRolePermissionDto
{
  public string Role { get; set; } = default!;
  public string Permission { get; set; } = default!;
}

public class CreateRolePermissionDtoValidator : AbstractValidator<CreateRolePermissionDto>
{
  public CreateRolePermissionDtoValidator()
  {
    RuleFor(dto => dto.Role)
      .NotEmpty()
      .Must(role => Role.List.Select(role => role.Value).Contains(role));

    RuleFor(dto => dto.Permission)
      .NotEmpty()
      .Must(permission => Permissions.ToList().Contains(permission!));
  }
}