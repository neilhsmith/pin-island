using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PinIsland.Api.Database.Converters;

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
  public DateOnlyConverter() : base(
    d => d.ToDateTime(TimeOnly.MinValue),
    d => DateOnly.FromDateTime(d))
  { }
}