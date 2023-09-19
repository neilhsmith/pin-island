using Microsoft.AspNetCore.Authorization;

namespace PinIsland.Api.Extensions.Services;

public static class AuthorizationPolicyBuilderExtensions
{
  public static void AddRequiredScopesPolicy(this AuthorizationOptions authorizationOptions, string policyName, params string[] scopes) =>
    authorizationOptions.AddPolicy(policyName, policyBuilder => policyBuilder.RequireScopes(scopes));

  public static AuthorizationPolicyBuilder RequireScopes(this AuthorizationPolicyBuilder builder, params string[] scopes) =>
    builder.RequireAssertion(context =>
      {
        var userScopes = GetUserScopes(context);
        return scopes.All(scope => userScopes.Contains(scope, StringComparer.CurrentCulture));
      });

  public static List<string> GetUserScopes(this AuthorizationHandlerContext context) =>
    context?.User?
      .Claims
      .Where(c => c.Type.Equals("scope"))
      .SelectMany(c => c.Value.Split(' ')).ToList() ?? new List<string>();
}
