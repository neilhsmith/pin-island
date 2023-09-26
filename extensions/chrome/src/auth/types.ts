export type AuthToken = {
  access_token: string
  expires_at: string
}

export type User = {
  email: string
  email_verified: boolean
  family_name: string
  given_name: string
  name: string
  preferred_username: string
  sub: string
  website: string
}
