import { useEffect } from "react"
import { MESSAGES } from "../../constants"

export default function App() {
  useEffect(() => {
    console.log("content view loaded")
  }, [])

  return (
    <div>
      <button
        className="text-lime-600 underline"
        onClick={() =>
          chrome.runtime.sendMessage({ type: MESSAGES.OPEN_SIDE_PANEL })
        }
      >
        Open Side Panel
      </button>
    </div>
  )
}
