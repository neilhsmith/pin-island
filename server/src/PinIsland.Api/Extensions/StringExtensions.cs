namespace PinIsland.Api.Extensions;

public static class StringExtensions
{
  public static string Sanatize(this string value)
  {
    return value.Replace(" ", "").ToLower();
  }
}