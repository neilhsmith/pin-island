namespace PinIsland.Api.Extensions;

public static class StringExtensions
{
  public static string Normalize(this string value)
  {
    return value.Replace(" ", "").ToLower();
  }
}