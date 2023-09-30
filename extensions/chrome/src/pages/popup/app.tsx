import logo from "../../assets/react.svg"
import { UserProvider } from "../../auth/context"
import { LoginLogoutButton } from "@/auth/buttons"

function App() {
  return (
    <UserProvider>
      <div className="bg-slate-50">
        <img
          src={chrome.runtime.getURL(logo)}
          className="App-logo"
          alt="logo"
        />
        <p className="text-8xl mb-10">sup bitch</p>
        <div className="bg-white p-2">
          <LoginLogoutButton />
        </div>
      </div>
    </UserProvider>
  )
}

export default App
