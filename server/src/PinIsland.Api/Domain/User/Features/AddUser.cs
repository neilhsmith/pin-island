using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;

namespace PinIsland.Api.Domain.User.Features;

public static class AddUser
{
  public sealed record Command(AddUserDto AddUserDto) : IRequest<UserDto>;

  public sealed class Handler : IRequestHandler<Command, UserDto>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
      var validator = new AddUserDtoValidator();
      validator.ValidateAndThrow(request.AddUserDto);

      var existingUserByIdentifier = await _dbContext.Users.FirstOrDefaultAsync(
        user => user.Identifier == request.AddUserDto.Identifier, cancellationToken);

      if (existingUserByIdentifier is not null)
      {
        // TODO: throw: the user w/ the idp's identifier already exists
      }

      var newUser = new User
      {
        Identifier = request.AddUserDto.Identifier,
        PreferredUsername = request.AddUserDto.PreferredUsername,
        EmailVerified = request.AddUserDto.EmailVerified,
        Email = request.AddUserDto.Email,
        FirstName = request.AddUserDto.FirstName,
        LastName = request.AddUserDto.LastName,
        Webpage = request.AddUserDto.Webpage,
      };

      await _dbContext.Users.AddAsync(newUser, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);

      return new UserDto
      {
        Id = newUser.Id,
        Identifier = newUser.Identifier,
        PreferredUsername = newUser.PreferredUsername,
        EmailVerified = newUser.EmailVerified,
        Email = newUser.Email,
        FirstName = newUser.FirstName,
        LastName = newUser.LastName,
        Webpage = newUser.Webpage,
      };
    }
  }
}

public class AddUserDto
{
  public string Identifier { get; set; } = default!;
  public string PreferredUsername { get; set; } = default!;
  public bool EmailVerified { get; set; }
  public string? Email { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Webpage { get; set; }
}

public class AddUserDtoValidator : AbstractValidator<AddUserDto>
{
  public AddUserDtoValidator()
  {
    RuleFor(u => u.Identifier)
      .NotEmpty()
      .MaximumLength(256);

    RuleFor(u => u.PreferredUsername)
      .NotEmpty()
      .MaximumLength(64);
  }
}