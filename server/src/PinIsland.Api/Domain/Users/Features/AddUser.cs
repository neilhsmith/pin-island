using FluentValidation;
using MediatR;
using PinIsland.Api.Database;

namespace PinIsland.Api.Domain.Users.Features;

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
      var validator = new AddUserValidator();
      validator.ValidateAndThrow(request.AddUserDto);

      var user = new User
      {
        Identifier = request.AddUserDto.Identifier,
        Username = request.AddUserDto.Username,
        Email = request.AddUserDto.Email,
        FirstName = request.AddUserDto.FirstName,
        LastName = request.AddUserDto.LastName,
        Website = request.AddUserDto.Website
      };

      await _dbContext.Users.AddAsync(user, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);

      var userDto = new UserDto
      {
        Id = user.Id,
        Identifier = user.Identifier,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Website = user.LastName
      };

      return userDto;
    }
  }

  private sealed class AddUserValidator : AbstractValidator<AddUserDto>
  {
    public AddUserValidator()
    {
      RuleFor(createUser => createUser.Identifier)
        .NotEmpty();

      RuleFor(createUser => createUser.Username)
        .NotEmpty()
        .MaximumLength(32);

      RuleFor(createUser => createUser.Email)
        .NotEmpty()
        .EmailAddress();

      RuleFor(createUser => createUser.FirstName)
        .MaximumLength(32);

      RuleFor(createUser => createUser.LastName)
        .MaximumLength(32);

      RuleFor(createUser => createUser.FirstName)
        .MaximumLength(256);
    }
  }
}

public class AddUserDto
{
  public string Identifier { get; set; } = default!;
  public string Username { get; set; } = default!;
  public string Email { get; set; } = default!;

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Website { get; set; }
}
