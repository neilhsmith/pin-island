import logo from "../../assets/react.svg"
import { UserProvider, useUser } from "../../auth/context"

function LoginButton() {
  const { login } = useUser()

  return <button onClick={login}>Login</button>
}

function LogoutButton() {
  const { logout } = useUser()

  return <button onClick={logout}>Logout</button>
}

function LoginOrLogout() {
  const { user } = useUser()

  return user ? <LogoutButton /> : <LoginButton />
}

function App() {
  return (
    <UserProvider>
      <img src={chrome.runtime.getURL(logo)} className="App-logo" alt="logo" />
      <p className="text-8xl mb-10">sup bitch</p>
      <div>
        <LoginOrLogout />
      </div>
    </UserProvider>
  )
}

export default App
