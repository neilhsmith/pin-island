import {
  PropsWithChildren,
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
} from "react"
import { UserInfo, authMessages } from "./types"

type Status = "loading" | "logged_in" | "logged_out"

type UserContextType = {
  status: Status
  user?: UserInfo | null

  login: () => void
  logout: () => void
}

const UserContext = createContext<UserContextType>(
  null as unknown as UserContextType
)

const abortController = new AbortController()

export const UserProvider = ({ children }: PropsWithChildren) => {
  const [status, setStatus] = useState<Status>("loading")
  const [user, setUser] = useState<UserInfo | null>()

  useEffect(() => {
    ;(async function () {
      const response = await chrome.runtime.sendMessage({
        type: authMessages.INIT,
        abortController,
      })

      if (response.status === "success") {
        setStatus(response.user ? "logged_in" : "logged_out")
        setUser(response.user)
      } else {
        // display error?
        setStatus("logged_out")
      }
    })()

    return () => abortController.abort()
  }, [])

  const login = useCallback(() => {
    ;(async function () {
      setStatus("loading")
      const response = await chrome.runtime.sendMessage({
        type: authMessages.LOGIN,
      })

      if (response.status === "success") {
        setStatus(response.user ? "logged_in" : "logged_out")
        setUser(response.user)
      } else {
        // display error?
        setStatus("logged_out")
      }
    })()
  }, [])

  const logout = useCallback(() => {
    ;(async function () {
      setStatus("loading")
      const response = await chrome.runtime.sendMessage({
        type: authMessages.LOGOUT,
      })

      if (response.status === "success") {
        setStatus("logged_out")
        setUser(null)
      } else {
        // TODO: how to handle errors?
        // think i need to know if we actually cleared the chrome storage and/or idp session
      }
    })()
  }, [])

  console.log("UserProvider status", status)
  return (
    <UserContext.Provider
      value={{
        status,
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
