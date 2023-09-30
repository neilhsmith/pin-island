import { Button } from "@/components/ui/button"
import { useUser } from "./context"

const LoginButton = () => {
  const { login } = useUser()
  return (
    <Button variant="outline" fluid onClick={login}>
      Login
    </Button>
  )
}

const LogoutButton = () => {
  const { logout } = useUser()
  return (
    <Button variant="default" fluid onClick={logout}>
      Logout
    </Button>
  )
}

export const LoginLogoutButton = () => {
  const { user } = useUser()
  return user ? <LogoutButton /> : <LoginButton />
}
