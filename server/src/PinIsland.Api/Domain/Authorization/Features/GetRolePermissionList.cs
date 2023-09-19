using MediatR;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Configurations;
using PinIsland.Api.Database;
using PinIsland.Api.Domain.Authorization.Dtos;
using QueryKit;
using QueryKit.Configuration;

namespace PinIsland.Api.Domain.Authorization.Features;

public static class GetRolePermissionList
{
  public sealed record Query(RolePermissionParametersDto RolePermissionParametersDto)
    : IRequest<PagedList<RolePermissionDto>>;

  public sealed class Handler : IRequestHandler<Query, PagedList<RolePermissionDto>>
  {
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<PagedList<RolePermissionDto>> Handle(Query request, CancellationToken cancellationToken)
    {

      var queryKitConfiguration = new AppQueryKitConfiguration();
      var queryKitData = new QueryKitData
      {
        Filters = request.RolePermissionParametersDto.Filters,
        SortOrder = request.RolePermissionParametersDto.SortOrder,
        Configuration = queryKitConfiguration
      };

      var collection = _dbContext.RolePermissions.AsNoTracking();
      var appliedCollection = collection.ApplyQueryKit(queryKitData);

      var rolePermissionDtos = appliedCollection.Select(rp => new RolePermissionDto
      {
        Id = rp.Id,
        Permission = rp.Permission,
        Role = new RoleDto
        {
          Name = rp.Role.Name,
          Value = rp.Role.Value
        }
      });

      return await PagedList<RolePermissionDto>.CreateAsync(
        rolePermissionDtos,
        request.RolePermissionParametersDto.PageNumber,
        request.RolePermissionParametersDto.PageSize,
        cancellationToken
      );
    }
  }
}

public sealed class RolePermissionParametersDto : BasePaginationParameters
{
  public string? Filters { get; set; }
  public string? SortOrder { get; set; }
}