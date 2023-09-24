import React from "react"
import ReactDOM from "react-dom/client"
import App from "./app"
import "./index.css"

const root = document.createElement("div")
root.id = "crx-root"
document.body.appendChild(root)

ReactDOM.createRoot(root).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)

console.log("content script loaded")
