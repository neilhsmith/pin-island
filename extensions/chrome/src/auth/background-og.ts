/**
 * TODO:
 * - [x] on UserProvider mount, validate the stored token (if one exists) and set initial state
 * - [ ] handle refreshing the token
 * - [ ] create an api util that gets & uses the current access_token to do a private fetch from the api
 * - [ ] handle revoked sessions
 * - [ ] clean this file up
 */

import { AuthToken, UserInfo } from "./types"

// handle login & logout requests
chrome.runtime.onMessage.addListener(
  (request, sender, sendResponse: (...args: unknown[]) => void) => {
    if (request.type === "login") {
      login().then((userInfo) => {
        sendResponse({
          status: "success",
          userInfo,
        })
      })
      return true
    } else if (request.type === "logout") {
      logout().then(() => {
        sendResponse({
          status: "success",
        })
      })
      return true
    } else if (request.type === "validate_token") {
      chrome.storage.local
        .get("authToken")
        .then(async (storage) => {
          let access_token = storage.authToken?.access_token
          if (Date.now() >= storage.authToken.access_token_expires_at) {
            const newAuthToken = await refreshAuthToken(storage.authToken)
            access_token = newAuthToken.access_token
          }
          return getUserInfo(access_token)
        })
        .then((userInfo) => {
          sendResponse({
            status: "success",
            userInfo,
          })
        })
      return true
    }

    return false
  }
)

async function login() {
  // generate state, pkce verifier/challenge, & url
  const state = randomstring()
  const codeVerifier = randomstring(128)
  const codeChallenge = await generateCodeChallenge(codeVerifier)
  const url = buildLoginUrl(codeChallenge, state)

  // iniate auth flow - silent if able
  const redirectUrl = await chrome.identity.launchWebAuthFlow({
    url,
    interactive: true,
    abortOnLoadForNonInteractive: true,
  })

  // validate the returned redirectUrl
  if (!redirectUrl || !redirectUrl.length)
    throw new Error("Unable to get the redirect_url.")
  const params = new URLSearchParams(redirectUrl)
  // FIXME: this is throwing when i don't expect it
  // const resState = params.get("session_state")
  // if (resState !== code_verifier)
  //   throw new Error("The returned state does not match the expected state.")
  const authCode = params.get("code")
  if (!authCode) throw new Error("Unable to get the code.")

  // exchange the code for an auth token
  const authToken = await getAuthToken(authCode, codeVerifier, state)

  // store the auth token
  await chrome.storage.local.set({ authToken })
  console.log("no big deal or anything but i got that authToken", authToken)

  // get the user info from the token
  const userInfo = await getUserInfo(authToken.access_token)

  // TODO: broadcast logged_in message
  // chrome.runtime.sendMessage({
  //   type: "auth:logged_in",
  //   userInfo,
  // })

  return userInfo
}

async function logout() {
  const authToken = await chrome.storage.local.get("authToken")

  // end the keycloak session then delete the auth token then return
  const res = await fetch(
    "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/logout",
    {
      headers: {
        Authorization: `bearer ${authToken.access_token}`,
      },
    }
  )

  if (!res.ok) throw new Error("Could not revoke session.")

  await chrome.storage.local.remove("authToken")

  return
}

async function getAuthToken(
  authCode: string,
  codeVerifier: string,
  state: string
) {
  const url = new URL(
    "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/token"
  )

  const params: Record<string, string> = {
    grant_type: "authorization_code",
    client_id: "pin-island.extensions.chrome",
    scope: "pin-island_api_read pin-island_api_write",
    state,
    code: authCode,
    code_verifier: codeVerifier,
    redirect_url: chrome.identity.getRedirectURL(),
  }

  for (const key in params) {
    url.searchParams.append(key, params[key])
  }

  const res = await fetch(`${url.origin}${url.pathname}`, {
    method: "POST",
    body: url.searchParams,
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
      Accept: "application/json",
    },
  })

  if (!res.ok) throw new Error(res.statusText)

  const result = await res.json()

  const now = Date.now()
  result.access_token_expires_at = now + (result.expires_in - 5) * 1000
  result.refresh_token_expires_at = now + (result.refresh_expires_in - 5) * 1000

  delete result.expires_in
  delete result.refresh_expires_in

  return result as AuthToken
}

async function refreshAuthToken(authToken: AuthToken) {
  const url = new URL(
    "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/token"
  )

  const params: Record<string, string> = {
    grant_type: "refresh_token",
    client_id: "pin-island.extensions.chrome",
    refresh_token: authToken.refresh_token,
  }

  for (const key in params) {
    url.searchParams.append(key, params[key])
  }

  const res = await fetch(`${url.origin}${url.pathname}`, {
    method: "POST",
    body: url.searchParams,
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
      Authorization: `bearer ${authToken.access_token}`,
    },
  })

  if (!res.ok) throw new Error(res.statusText)

  const result = await res.json()

  const now = Date.now()
  result.access_token_expires_at = now + (result.expires_in - 5) * 1000
  result.refresh_token_expires_at = now + (result.refresh_expires_in - 5) * 1000

  delete result.expires_in
  delete result.refresh_expires_in

  await chrome.storage.local.remove("authToken")
  await chrome.storage.local.set({ authToken: result })

  return result as AuthToken
}

async function getUserInfo(accessToken: string) {
  const res = await fetch(
    "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/userinfo",
    {
      headers: {
        Authorization: `bearer ${accessToken}`,
      },
    }
  )

  if (!res.ok) throw new Error("Could not get UserInfo.")

  return (await res.json()) as UserInfo
}

async function generateCodeChallenge(codeVerifier: string) {
  const encoder = new TextEncoder()
  const data = encoder.encode(codeVerifier)
  const digest = await crypto.subtle.digest("SHA-256", data)
  const base64Digest = btoa(String.fromCharCode(...new Uint8Array(digest)))

  return base64Digest.replace(/\+/g, "-").replace(/\//g, "_").replace(/=/g, "")
}

function buildLoginUrl(codeChallenge: string, state: string) {
  const url = new URL(
    "http://localhost:5454/auth/realms/pin-island/protocol/openid-connect/auth"
  )

  const params: Record<string, string> = {
    response_type: "code",
    client_id: "pin-island.extensions.chrome",
    scope: "pin-island_api_read pin-island_api_write",
    state: state,
    code_challenge: codeChallenge,
    code_challenge_method: "S256",
    redirect_url: chrome.identity.getRedirectURL(),
  }

  for (const key in params) {
    url.searchParams.append(key, params[key])
  }

  return url.toString()
}

function randomstring(length: number = 32) {
  let result = ""
  const characters =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
  const charactersLength = characters.length
  let counter = 0
  while (counter < length) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength))
    counter += 1
  }
  return result
}
