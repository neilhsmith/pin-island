export type AuthToken = {
  access_token: string
  access_token_expires_at: number
  refresh_token: string
  refresh_token_expires_at: number
  scope: string
  token_type: string
}

export type UserInfo = {
  email: string
  email_verified: boolean
  family_name: string
  given_name: string
  name: string
  preferred_username: string
  sub: string
  website: string
}

export const authMessages = {
  INIT: "auth:init",
  LOGIN: "auth:login",
  LOGOUT: "auth:logout",
} as const

export type AuthMessageResponsePayload =
  | {
      status: "success"
      user?: UserInfo
    }
  | {
      status: "error"
      error?: unknown
    }
