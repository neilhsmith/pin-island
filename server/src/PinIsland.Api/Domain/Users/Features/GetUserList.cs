using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Services;
using QueryKit;
using QueryKit.Configuration;

namespace PinIsland.Api.Domain.Users.Features;

public static class GetUserList
{
  public sealed record Query(GetUserListDto GetUserListDto) : IRequest<PagedList<UserDto>>;

  public sealed class Handler : IRequestHandler<Query, PagedList<UserDto>>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<PagedList<UserDto>> Handle(Query request, CancellationToken cancellationToken)
    {
      var collection = _dbContext.Users.AsNoTracking();

      var queryKitConfig = new AppQueryKitConfiguration();
      var queryKitData = new QueryKitData
      {
        Filters = request.GetUserListDto.Filters,
        SortOrder = request.GetUserListDto.SortOrder,
        Configuration = queryKitConfig
      };
      var appliedCollection = collection.ApplyQueryKit(queryKitData);
      var dtoCollection = appliedCollection.Select(user => new UserDto
      {
        Id = user.Id,
        Identifier = user.Identifier,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Website = user.LastName
      });

      return await PagedList<UserDto>.CreateAsync(
        dtoCollection,
        request.GetUserListDto.PageNumber,
        request.GetUserListDto.PageSize,
        cancellationToken
      );
    }
  }
}

public sealed class GetUserListDto : BasePaginationParameters
{
  public string? Filters { get; set; } = default!;
  public string? SortOrder { get; set; } = default!;
}