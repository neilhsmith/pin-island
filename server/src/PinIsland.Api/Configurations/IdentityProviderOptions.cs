namespace PinIsland.Api.Configurations;

public class IdentityProviderOptions
{
  public const string SectionName = "IdentityProvider";

  public string Audience { get; set; } = string.Empty;
  public string Authority { get; set; } = string.Empty;

  public string ReadScope { get; set; } = string.Empty;
  public string WriteScope { get; set; } = string.Empty;
}

public static class IdentityProviderOptionsExtensions
{
  public static IdentityProviderOptions GetIdentityProviderOptions(this IConfiguration configuration)
    => configuration.GetSection(IdentityProviderOptions.SectionName).Get<IdentityProviderOptions>()
      ?? new IdentityProviderOptions();
}