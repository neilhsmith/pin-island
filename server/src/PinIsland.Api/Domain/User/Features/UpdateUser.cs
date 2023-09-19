using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;

namespace PinIsland.Api.Domain.User.Features;

public static class UpdateUser
{
  public sealed record Command(Guid Id, UpdateUserDto UpdateUserDto) : IRequest<UserDto>;

  public sealed class Handler : IRequestHandler<Command, UserDto>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
      var validator = new UpdateUserDtoValidator();
      validator.ValidateAndThrow(request.UpdateUserDto);

      var existingUser = await _dbContext.Users.FirstAsync(
        user => user.Id == request.Id, cancellationToken);

      existingUser.Identifier = request.UpdateUserDto.Identifier;
      existingUser.PreferredUsername = request.UpdateUserDto.PreferredUsername;
      existingUser.EmailVerified = request.UpdateUserDto.EmailVerified;
      existingUser.Email = request.UpdateUserDto.Email;
      existingUser.FirstName = request.UpdateUserDto.FirstName;
      existingUser.LastName = request.UpdateUserDto.LastName;
      existingUser.Webpage = request.UpdateUserDto.Webpage;

      _dbContext.Users.Update(existingUser);
      await _dbContext.SaveChangesAsync();

      return new UserDto
      {
        Id = existingUser.Id,
        Identifier = existingUser.Identifier,
        PreferredUsername = existingUser.PreferredUsername,
        EmailVerified = existingUser.EmailVerified,
        Email = existingUser.Email,
        FirstName = existingUser.FirstName,
        LastName = existingUser.LastName,
        Webpage = existingUser.Webpage,
      };
    }
  }
}

public class UpdateUserDto
{
  public string Identifier { get; set; } = default!;
  public string PreferredUsername { get; set; } = default!;
  public bool EmailVerified { get; set; }
  public string? Email { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Webpage { get; set; }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
  public UpdateUserDtoValidator()
  {
    RuleFor(u => u.Identifier)
      .NotEmpty()
      .MaximumLength(256);

    RuleFor(u => u.PreferredUsername)
      .NotEmpty()
      .MaximumLength(64);
  }
}