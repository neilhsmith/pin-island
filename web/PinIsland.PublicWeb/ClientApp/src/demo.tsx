import { Button } from "@/components/ui/button"
import { useEffect, useState } from "react"

type Claim = {
  type: string
  value: string
}

export const Demo = () => {
  const [logoutUrl, setLogoutUrl] = useState("/bff/logout")
  const [claims, setClaims] = useState<Claim[]>([])
  const [response, setResponse] = useState<unknown>()

  useEffect(() => {
    onLoad()
    async function onLoad() {
      const req = new Request("/bff/user", {
        headers: new Headers({
          "X-CSRF": "1",
        }),
      })

      try {
        const resp = await fetch(req)
        if (resp.ok) {
          const claims = (await resp.json()) as Claim[]
          setClaims(claims)

          if (claims) {
            console.log("user logged in")

            const logoutUrlClaim = claims.find(
              (claim) => claim.type === "bff:logout_url"
            )
            if (logoutUrlClaim) {
              setLogoutUrl(logoutUrlClaim.value)
            }
          } else {
            console.log("user not logged in")
          }
        } else if (resp.status === 401) {
          console.log("user not logged in")

          const silentLoginResult = await silentLogin()
          console.log("silent login result: " + silentLoginResult)

          if (silentLoginResult) {
            window.location.reload()
          }
        }
      } catch (e) {
        console.error(e)
      }
    }
  }, [])

  const doFetch = async (url: string) => {
    const req = new Request(url, {
      headers: new Headers({
        "X-CSRF": "1",
      }),
    })

    const resp = await fetch(req)

    console.log("API Result: " + resp.status)

    if (resp.ok) {
      const result = await resp.json()
      setResponse(result)
    } else {
      setResponse({
        status: resp.status,
        statusText: resp.statusText,
      })
    }
  }

  return (
    <div className="bg-slate-100 min-h-screen">
      <div className="container flex flex-col">
        <h1 className="text-xl font-bold block border-b border-b-slate-300">
          Demo
        </h1>
        <div className="border-b border-b-slate-300 py-4">
          <ul className="flex gap-4">
            <li>
              <Button variant="outline" asChild>
                <a href="/bff/login">Login</a>
              </Button>
            </li>
            <li>
              <Button variant="outline" asChild>
                <a href={logoutUrl}>Logout</a>
              </Button>
            </li>
            <li>
              <Button>Local Public</Button>
            </li>
            <li>
              <Button>Local Private</Button>
            </li>
            <li>
              <Button onClick={() => doFetch("/api/test/public")}>
                Remote Public
              </Button>
            </li>
            <li>
              <Button onClick={() => doFetch("/api/test/private")}>
                Remote Private
              </Button>
            </li>
          </ul>
        </div>
        <div className="py-4">
          <div className="flex gap-8">
            <div className="border border-slate-300 flex-grow w-full rounded p-4">
              <h2 className="text-lg font-semibold">Claims</h2>
              <pre className="text-sm">{JSON.stringify(claims, null, 2)}</pre>
            </div>
            <div className="border border-slate-300 flex-grow w-full rounded p-4">
              <h2 className="text-lg font-semibold">Fetch Result</h2>
              <pre className="text-sm">
                {response ? JSON.stringify(response, null, 2) : "-"}
              </pre>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

function silentLogin(
  silentLoginUrl: string = "/bff/silent-login",
  iframeSelector: string = "#bff-silent-login"
) {
  const timeout = 5000

  return new Promise((resolve) => {
    function onMessage(e: MessageEvent) {
      // look for messages sent from the BFF iframe
      if (e.data && e.data["source"] === "bff-silent-login") {
        window.removeEventListener("message", onMessage)
        // send along the boolean result
        resolve(e.data.isLoggedIn)
      }
    }

    // listen for the iframe response to notify its parent (i.e. this window).
    window.addEventListener("message", onMessage)

    // we're setting up a time to handle scenarios when the iframe doesn't return immediately (despite prompt=none).
    // this likely means the iframe is showing the error page at IdentityServer (typically due to client misconfiguration).
    window.setTimeout(() => {
      window.removeEventListener("message", onMessage)

      // we can either just treat this like a "not logged in"
      resolve(false)
      // or we can trigger an error, so someone can look into the reason why
      // reject(new Error("timed_out"));
    }, timeout)

    // send the iframe to the silent login endpoint to kick off the workflow
    const frame = document.querySelector<HTMLIFrameElement>(iframeSelector)
    if (frame) {
      frame.src = silentLoginUrl
    }
  })
}
