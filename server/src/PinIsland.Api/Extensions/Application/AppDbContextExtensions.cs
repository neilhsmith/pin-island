using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PinIsland.Api.Domain;

namespace PinIsland.Api.Extensions.Application;

public static class AppDbContextExtensions
{
  public static void FilterSoftDeletedRecords(this ModelBuilder modelBuilder)
  {
    Expression<Func<BaseEntity, bool>> filterExpression = e => !e.IsDeleted;

    foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes()
      .Where(m => m.ClrType.IsAssignableTo(typeof(BaseEntity))))
    {
      var parameter = Expression.Parameter(mutableEntityType.ClrType);
      var body = ReplacingExpressionVisitor
        .Replace(filterExpression.Parameters.First(), parameter, filterExpression.Body);
      var lambdaExpression = Expression.Lambda(body, parameter);

      mutableEntityType.SetQueryFilter(lambdaExpression);
    }
  }
}