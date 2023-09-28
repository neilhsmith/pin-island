using Duende.Bff;

namespace PinIsland.PublicWeb;

public class Configuration
{
  public string? Authority { get; set; }

  public string? ClientId { get; set; }

  public string? ClientSecret { get; set; }

  public List<string> Scopes { get; set; } = new();
  public List<Api> Apis { get; set; } = new();
}

public class Api
{
  public string? LocalPath { get; set; }
  public string? RemoteUrl { get; set; }
}