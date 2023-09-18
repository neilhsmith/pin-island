namespace PinIsland.Api.Configurations;

public class SwaggerAuthClientOptions
{
  public const string SectionName = "SwaggerAuthClient";

  public string Audience { get; set; } = string.Empty;
  public string Authority { get; set; } = string.Empty;
  public string AuthorizationUrl { get; set; } = string.Empty;
  public string TokenUrl { get; set; } = string.Empty;
  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
}

public static class SwaggerAuthClientOptionsExtensions
{
  public static SwaggerAuthClientOptions GetSwaggerAuthClientOptions(this IConfiguration configuration)
    => configuration.GetSection(SwaggerAuthClientOptions.SectionName).Get<SwaggerAuthClientOptions>()
      ?? new SwaggerAuthClientOptions();
}