using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Database;
using PinIsland.Api.Services;
using QueryKit;
using QueryKit.Configuration;

namespace PinIsland.Api.Domain.Roles.Features;

public static class GetRoleList
{
  public sealed record Query(GetRoleListDto GetRoleListDto) : IRequest<PagedList<RoleDto>>;

  public sealed class Handler : IRequestHandler<Query, PagedList<RoleDto>>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<PagedList<RoleDto>> Handle(Query request, CancellationToken cancellationToken)
    {
      var collection = _dbContext.Roles.AsNoTracking();

      var queryKitConfig = new AppQueryKitConfiguration();
      var queryKitData = new QueryKitData
      {
        Filters = request.GetRoleListDto.Filters,
        SortOrder = request.GetRoleListDto.SortOrder,
        Configuration = queryKitConfig
      };
      var appliedCollection = collection.ApplyQueryKit(queryKitData);
      var dtoCollection = appliedCollection.Select(role => new RoleDto
      {
        Id = role.Id,
        Name = role.Name
      });

      return await PagedList<RoleDto>.CreateAsync(
        dtoCollection,
        request.GetRoleListDto.PageNumber,
        request.GetRoleListDto.PageSize,
        cancellationToken
      );
    }
  }
}

public sealed class GetRoleListDto : BasePaginationParameters
{
  public string? Filters { get; set; } = default!;
  public string? SortOrder { get; set; } = default!;
}