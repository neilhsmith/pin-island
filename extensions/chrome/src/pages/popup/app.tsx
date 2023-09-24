import logo from "../../assets/react.svg"

function App() {
  return (
    <div className="text-8xl">
      <img src={chrome.runtime.getURL(logo)} className="App-logo" alt="logo" />
      <p>sup bitch</p>
    </div>
  )
}

export default App
