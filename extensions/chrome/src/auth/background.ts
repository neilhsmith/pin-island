import { oidcSettings } from "./config"
import {
  AuthMessageResponsePayload,
  AuthToken,
  UserInfo,
  authMessages,
} from "./types"

/**
 * TODO:
 * - [ ] consolidate calculating expiration_ats in both login & refresh
 * - [ ] cleanup / document which fns get/update storage
 *    - maybe move the pure fns to a util file?
 * - [ ] create a type system for message types/payloads and responses
 * - [ ] figure out how the api util will work and get the token
 *    - create a demo which fetches from the private test endpoint
 * - [ ] figure out how the api util will refresh when the access_token expires
 *    - make sure the private test endpoint works across refreshes
 */

chrome.runtime.onMessage.addListener(
  (request, _, sendResponse: (payload: AuthMessageResponsePayload) => void) => {
    try {
      switch (request.type) {
        case authMessages.INIT:
          init(request.abortController).then((user) =>
            sendResponse({
              status: "success",
              user,
            })
          )
          return true
        case authMessages.LOGIN:
          login(true).then((user) =>
            sendResponse({
              status: "success",
              user,
            })
          )
          return true
        case authMessages.LOGOUT:
          logout().then(() =>
            sendResponse({
              status: "success",
            })
          )
          return true
      }
    } catch (error) {
      sendResponse({
        status: "error",
        error,
      })
      return true
    }

    return false
  }
)

async function init(abortController: AbortController) {
  console.log("init")

  let authToken = await getStoredAuthToken()
  if (!authToken) return

  const now = Date.now()
  if (now >= authToken.access_token_expires_at) {
    console.log("access_token expired", authToken, now)
    if (now >= authToken.refresh_token_expires_at) {
      console.log("refresh_token expired", authToken, now)
      return await login(false)
    } else {
      authToken = await refreshAuthToken(abortController)
    }
  }

  return fetchUserInfo(authToken, abortController)
}

async function login(interactive: boolean): Promise<UserInfo | undefined> {
  console.log("login")

  await clearStoredAuthToken()

  const state = randomstring()
  const codeVerifier = randomstring(128)
  const codeChallenge = await generateCodeChallenge(codeVerifier)

  const loginUrl = new URL(oidcSettings.authorizationUrl)
  const loginUrlParams: Record<string, string> = {
    response_type: "code",
    client_id: oidcSettings.clientId,
    scope: oidcSettings.scope,
    state: state,
    code_challenge: codeChallenge,
    code_challenge_method: "S256",
    redirect_url: chrome.identity.getRedirectURL(),
  }
  for (const key in loginUrlParams) {
    loginUrl.searchParams.append(key, loginUrlParams[key])
  }

  const redirectUrl = await chrome.identity.launchWebAuthFlow({
    url: loginUrl.toString(),
    interactive,
    abortOnLoadForNonInteractive: true,
  })

  if (!redirectUrl || !redirectUrl.length)
    throw new Error("Unable to get the redirect_url.")

  const params = new URLSearchParams(redirectUrl)
  const code = params.get("code")
  if (!code) throw new Error("Unable to get the authorization code.")

  const authToken = await fetchAuthToken(code, codeVerifier, state)
  await storeAuthToken(authToken)

  return fetchUserInfo(authToken)
}

async function logout(): Promise<void> {
  console.log("logout")

  const authToken = await getStoredAuthToken()

  if (!authToken) throw new Error("Cannot logout without an access_token.")

  const res = await fetch(oidcSettings.logoutUrl, {
    headers: {
      Authorization: `bearer ${authToken.access_token}`,
    },
  })

  if (!res.ok) throw new Error("Could not revoke session.")

  await clearStoredAuthToken()
}

async function refreshAuthToken(
  abortController: AbortController
): Promise<AuthToken> {
  console.log("refreshAuthToken")

  const authToken = await getStoredAuthToken()
  if (!authToken)
    throw new Error(
      "Cannot refresh token without an access_token and refresh_token."
    )

  const tokenUrl = new URL(oidcSettings.tokenUrl)
  const params: Record<string, string> = {
    grant_type: "refresh_token",
    client_id: oidcSettings.clientId,
    refresh_token: authToken.refresh_token,
  }

  for (const key in params) {
    tokenUrl.searchParams.append(key, params[key])
  }

  const res = await fetch(`${tokenUrl.origin}${tokenUrl.pathname}`, {
    method: "POST",
    body: tokenUrl.searchParams,
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
      Authorization: `bearer ${authToken.access_token}`,
    },
    signal: abortController.signal,
  })

  if (!res.ok) throw new Error(res.statusText)

  const result = await res.json()

  const now = Date.now()
  result.access_token_expires_at = now + (result.expires_in - 5) * 1000
  result.refresh_token_expires_at = now + (result.refresh_expires_in - 5) * 1000

  // delete result.expires_in
  // delete result.refresh_expires_in

  await storeAuthToken(result)
  return result as AuthToken
}

// ---

async function fetchAuthToken(
  authCode: string,
  codeVerifier: string,
  state: string
) {
  console.log("fetchAuthToken")

  const url = new URL(oidcSettings.tokenUrl)

  const params: Record<string, string> = {
    grant_type: "authorization_code",
    client_id: oidcSettings.clientId,
    scope: oidcSettings.scope,
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

  console.log("-----")
  console.log(
    "secs to access_token expiration: ",
    (result.access_token_expires_at - now) / 1000
  )
  console.log(
    "secs to refresh_token expiration: ",
    (result.refresh_token_expires_at - now) / 1000
  )
  console.log("-----")

  //delete result.expires_in
  //delete result.refresh_expires_in

  return result as AuthToken
}

async function fetchUserInfo(
  authToken: AuthToken,
  abortController?: AbortController
): Promise<UserInfo> {
  console.log("fetchUserInfo")

  if (!authToken)
    throw new Error("Cannot fetch user info without an access_token.")

  const res = await fetch(oidcSettings.userInfoUrl, {
    headers: {
      Authorization: `bearer ${authToken.access_token}`,
    },
    signal: abortController?.signal,
  })

  if (!res.ok) throw new Error("Could not get UserInfo.")

  return (await res.json()) as UserInfo
}

// utils

const STORED_AUTHTOKEN_KEY = "key:authToken"

async function getStoredAuthToken() {
  const result = await chrome.storage.local.get(STORED_AUTHTOKEN_KEY)

  if (typeof result[STORED_AUTHTOKEN_KEY] === "undefined") return undefined

  return result[STORED_AUTHTOKEN_KEY] as AuthToken
}

async function clearStoredAuthToken() {
  await chrome.storage.local.remove(STORED_AUTHTOKEN_KEY)
}

async function storeAuthToken(authToken: AuthToken) {
  await chrome.storage.local.set({
    [STORED_AUTHTOKEN_KEY]: authToken,
  })
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

async function generateCodeChallenge(codeVerifier: string) {
  const encoder = new TextEncoder()
  const data = encoder.encode(codeVerifier)
  const digest = await crypto.subtle.digest("SHA-256", data)
  const base64Digest = btoa(String.fromCharCode(...new Uint8Array(digest)))

  return base64Digest.replace(/\+/g, "-").replace(/\//g, "_").replace(/=/g, "")
}
