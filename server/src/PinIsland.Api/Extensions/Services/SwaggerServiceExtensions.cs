using System.Reflection;
using PinIsland.Api.Configurations;
using Microsoft.OpenApi.Models;

namespace PinIsland.Api.Extensions.Services;

public static class SwaggerServiceExtension
{
  public static void AddSwaggerExtension(this IServiceCollection services, IConfiguration configuration)
  {
    var swaggerAuthClientOptions = configuration.GetSwaggerAuthClientOptions();

    services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc("v1", new OpenApiInfo
      {
        Version = "v1",
        Title = "Pin Island API",
      });

      options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
      {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
          AuthorizationCode = new OpenApiOAuthFlow
          {
            AuthorizationUrl = new Uri(swaggerAuthClientOptions.AuthorizationUrl),
            TokenUrl = new Uri(swaggerAuthClientOptions.TokenUrl),
            Scopes = new Dictionary<string, string>
            {
              {"pin-island_api_read", "Pin Island API Read Access"},
              {"pin-island_api_write", "Pin Island API Write Access"}
            }
          }
        }
      });

      options.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
              Reference = new OpenApiReference
              {
                  Type = ReferenceType.SecurityScheme,
                  Id = "oauth2"
              },
              Scheme = "oauth2",
              Name = "oauth2",
              In = ParameterLocation.Header
          },
          new List<string>()
        }
      });

      var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });
  }
}