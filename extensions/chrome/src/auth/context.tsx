import {
  PropsWithChildren,
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
} from "react"
import { AuthToken, User } from "./types"

type Status = "idle" | "loading" | "logged_in" | "logged_out"

type UserContextType = {
  status: Status
  authToken?: AuthToken | null
  user?: User | null

  login: () => void
  logout: () => void
}

const UserContext = createContext<UserContextType>(
  null as unknown as UserContextType
)

export const UserProvider = ({ children }: PropsWithChildren) => {
  const [status, setStatus] = useState<Status>("idle")
  const [authToken, setAuthToken] = useState<AuthToken | null>()
  const [user, setUser] = useState<User | null>()

  useEffect(() => {
    soDo()
    async function soDo() {
      const response = await chrome.runtime.sendMessage({
        type: "login",
        interactive: false,
      })

      if (response.status === "success") {
        setStatus("logged_in")
        setAuthToken(response.authToken)
        setUser(response.user)
      } else {
        // TODO: handle error
        setStatus("logged_out")
      }
    }
  }, [])

  const login = useCallback(async () => {
    setStatus("loading")
    const response = await chrome.runtime.sendMessage({ type: "login" })

    if (response.status === "success") {
      setStatus("logged_in")
      setAuthToken(response.authToken)
      setUser(response.user)
    } else {
      // TODO: handle error
      setStatus("logged_out")
    }
  }, [])

  const logout = useCallback(async () => {
    setStatus("loading")
    const response = await chrome.runtime.sendMessage({ type: "logout" })
    setStatus("logged_out")

    if (response.status === "success") {
      setAuthToken(null)
      setUser(null)
    } else {
      // TODO: handle error
    }
  }, [])

  return (
    <UserContext.Provider
      value={{
        status,
        authToken,
        user,
        login,
        logout,
      }}
    >
      {children}
    </UserContext.Provider>
  )
}

export const useUser = () => useContext(UserContext)
