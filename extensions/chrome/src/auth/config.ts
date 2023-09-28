export const oidcSettings = {
  authorizationUrl:
    "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/auth",
  logoutUrl:
    "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/logout",
  tokenUrl:
    "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/token",
  userInfoUrl:
    "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/userinfo",
  clientId: "pin-island.extensions.chrome",
  scope: "pin-island_api_read pin-island_api_write",
} as const
