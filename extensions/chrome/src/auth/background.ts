/**
 * TODO:
 * - [x] on UserProvider mount, validate the stored token (if one exists) and set initial state
 * - [ ] create an api util that gets & uses the current access_token to do a private fetch from the api
 * - [ ] handle revoked sessions
 * - [ ] clean this file up
 */

// handle login & logout requests
chrome.runtime.onMessage.addListener(
  (request, sender, sendResponse: (...args: unknown[]) => void) => {
    if (request.type === "login") {
      chrome.identity.launchWebAuthFlow(
        {
          url: `http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/auth?response_type=token&client_id=pin-island.extensions.chrome&scope=pin-island_api_read pin-island_api_write&redirect_uri=${chrome.identity.getRedirectURL()}`,
          interactive: request.interactive ?? true,
          abortOnLoadForNonInteractive: request.interactive ?? false,
        },
        (redirectUrl) => {
          if (redirectUrl) {
            console.log("launchWebAuthFlow successful", redirectUrl)

            const params = new URLSearchParams(redirectUrl)
            const accessToken = params.get("access_token")
            const expiresInSeconds = params.get("expires_in")

            if (!accessToken || !expiresInSeconds) {
              throw new Error("TODO: error message")
            }

            const expiresAt = Date.now() + parseInt(expiresInSeconds) * 1000

            fetch(
              "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/userinfo",
              {
                headers: {
                  Authorization: `bearer ${accessToken}`,
                },
              }
            )
              .then((res) => res.json())
              .then(async (json) => {
                await chrome.storage.local.set({
                  access_token: accessToken,
                  access_token_expires_at: expiresAt,
                })
                sendResponse({
                  status: "success",
                  authToken: {
                    access_token: accessToken,
                    expires_at: expiresAt,
                  },
                  user: json,
                })
              })
              .catch(() => {
                sendResponse({
                  status: "failure",
                  error: "TODO: error message",
                })
              })
          } else {
            console.error("launchWebAuthFlow falied")
            sendResponse({
              status: "error",
              error: "TODO: error message",
            })
          }
        }
      )

      return true
    } else if (request.type === "logout") {
      // get the access token from storage
      // send a request to remove the session
      // if successful, delete everything for storage and sendReponse that we were successful
      // if error, sendResponse that we failed
      chrome.storage.local.get(["access_token"]).then((accessToken) => {
        fetch(
          "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/logout",
          {
            headers: {
              Authorization: `bearer ${accessToken}`,
            },
          }
        )
          .then(async (res) => {
            if (res.status !== 200) {
              throw new Error("TODO: error message")
            }

            await chrome.storage.local.remove([
              "access_token",
              "access_token_expires_at",
            ])
            sendResponse({
              status: "success",
            })
          })
          .catch(() => {
            console.error("End Session failed")
            sendResponse({
              status: "error",
              error: "TODO: error message",
            })
          })
      })

      return true
    }

    return false
  }
)
