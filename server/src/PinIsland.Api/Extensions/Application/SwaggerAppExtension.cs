using PinIsland.Api.Configurations;

namespace Linkster.Api.Extensions.Application;

public static class SwaggerAppExtension
{
  public static void UseSwaggerExtension(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI(options =>
      {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;

        var swaggerAuthClientOptions = configuration.GetSwaggerAuthClientOptions();
        options.OAuthClientId(swaggerAuthClientOptions.ClientId);
        options.OAuthClientSecret(swaggerAuthClientOptions.ClientSecret);
        options.OAuthUsePkce();
      });
    }
  }
}