using QueryKit.Configuration;

namespace PinIsland.Api.Configurations;

public class AppQueryKitConfiguration : QueryKitConfiguration
{
  public AppQueryKitConfiguration(Action<QueryKitSettings>? configureSettings = null)
    : base(settings =>
    {
      configureSettings?.Invoke(settings);
    })
  { }
}