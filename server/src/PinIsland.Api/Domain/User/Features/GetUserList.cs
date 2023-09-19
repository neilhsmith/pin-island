using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Configurations;
using PinIsland.Api.Database;
using PinIsland.Api.Domain.User;
using QueryKit;
using QueryKit.Configuration;

namespace PinIsland.Api.Domain.Users.Features;

public static class GetUserList
{
  public sealed record Query(UserParametersDto UserParametersDto)
    : IRequest<PagedList<UserDto>>;

  public sealed class Handler : IRequestHandler<Query, PagedList<UserDto>>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<PagedList<UserDto>> Handle(Query request, CancellationToken cancellationToken)
    {

      var queryKitConfiguration = new AppQueryKitConfiguration();
      var queryKitData = new QueryKitData
      {
        Filters = request.UserParametersDto.Filters,
        SortOrder = request.UserParametersDto.SortOrder,
        Configuration = queryKitConfiguration
      };

      var collection = _dbContext.Users.AsNoTracking();
      var appliedCollection = collection.ApplyQueryKit(queryKitData);

      var userDtos = appliedCollection.Select(user => new UserDto
      {
        Id = user.Id,
        Identifier = user.Identifier,
        PreferredUsername = user.PreferredUsername,
        EmailVerified = user.EmailVerified,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Webpage = user.Webpage,
      });

      return await PagedList<UserDto>.CreateAsync(
        userDtos,
        request.UserParametersDto.PageNumber,
        request.UserParametersDto.PageSize,
        cancellationToken
      );
    }
  }
}

public sealed class UserParametersDto : BasePaginationParameters
{
  public string? Filters { get; set; }
  public string? SortOrder { get; set; }
}