namespace Linkster.Api.Configurations;

public class ConnectionStringOptions
{
  public const string SectionName = "ConnectionStrings";
  public const string LinksterApiDevDbKey = nameof(LinksterApiDevDb);

  public string LinksterApiDevDb { get; set; } = string.Empty;
}

public static class ConnectionStringOptionsExtensions
{
  public static ConnectionStringOptions GetConnectionStringOptions(this IConfiguration configuration)
    => configuration.GetSection(ConnectionStringOptions.SectionName).Get<ConnectionStringOptions>()
      ?? new ConnectionStringOptions();
}