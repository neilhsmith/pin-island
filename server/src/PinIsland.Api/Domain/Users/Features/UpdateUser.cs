using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;

namespace PinIsland.Api.Domain.Users.Features;

public static class UpdateUser
{
  public sealed record Command(Guid Id, UpdateUserDto UpdateUserDto) : IRequest;

  public sealed class Handler : IRequestHandler<Command>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
      var validator = new UpdateUserValidator();
      validator.ValidateAndThrow(request.UpdateUserDto);

      var user = await _dbContext.Users.FirstAsync(user => user.Id == request.Id);

      user.Identifier = request.UpdateUserDto.Identifier;
      user.Username = request.UpdateUserDto.Username;
      user.Email = request.UpdateUserDto.Email;
      user.FirstName = request.UpdateUserDto.FirstName;
      user.LastName = request.UpdateUserDto.LastName;
      user.Website = request.UpdateUserDto.Website;

      _dbContext.Users.Update(user);
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
  }

  private sealed class UpdateUserValidator : AbstractValidator<UpdateUserDto>
  {
    public UpdateUserValidator()
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

public class UpdateUserDto
{
  public string Identifier { get; set; } = default!;
  public string Username { get; set; } = default!;
  public string Email { get; set; } = default!;

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Website { get; set; }
}
