using System.Reflection;
using System.Text.Json.Serialization;
using Linkster.Api.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.EntityFrameworkCore;
using PinIsland.Api.Configurations;
using PinIsland.Api.Database;
using PinIsland.Api.Services;

namespace PinIsland.Api.Extensions.Services;

public static class WebAppServiceExtensions
{
  public static void ConfigureServices(this WebApplicationBuilder builder)
  {
    var connectionString = builder.Configuration.GetConnectionStringOptions().LinksterApiDevDb;
    builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

    var idpOptions = builder.Configuration.GetIdentityProviderOptions();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
        {
          options.Authority = idpOptions.Authority;
          options.Audience = idpOptions.Audience;
          options.RequireHttpsMetadata = false;
          options.TokenValidationParameters.ValidTypes = new[] { "JWT" };
        });
    builder.Services.AddAuthorization(options =>
    {
      options.AddRequiredScopesPolicy("read_access", idpOptions.ReadScope);
      options.AddRequiredScopesPolicy("write_access", idpOptions.WriteScope);
    });

    builder.Services.AddControllers()
      .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    builder.Services.AddApiVersioning(config =>
    {
      config.AssumeDefaultVersionWhenUnspecified = true;
      config.ReportApiVersions = true;
      config.Conventions.Add(new VersionByNamespaceConvention());
    });

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddMediatR(config => config
      .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    builder.Services.AddMediatR(config => config
      .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    // registers all services that inherit from your base service interface - IPinIslandApiScopedService
    builder.Services.AddBoundaryServices(Assembly.GetExecutingAssembly());

    builder.Services.AddHealthChecks();
    builder.Services.AddSwaggerExtension(builder.Configuration);
  }

  /// <summary>
  /// Registers all services in the assembly of the given interface.
  /// </summary>
  private static void AddBoundaryServices(this IServiceCollection services, params Assembly[] assemblies)
  {
    if (!assemblies.Any())
      throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");

    foreach (var assembly in assemblies)
    {
      var rules = assembly.GetTypes()
          .Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(nameof(IPinIslandApiScopedService)) == typeof(IPinIslandApiScopedService));

      foreach (var rule in rules)
      {
        foreach (var @interface in rule.GetInterfaces())
        {
          services.Add(new ServiceDescriptor(@interface, rule, ServiceLifetime.Scoped));
        }
      }
    }
  }
}