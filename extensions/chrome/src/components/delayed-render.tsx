import { PropsWithChildren, useEffect, useState } from "react"

type DelayedLoadingContainerProps = {
  delay?: number
}

export const DelayedRender = ({
  children,
  delay = 500,
}: PropsWithChildren<DelayedLoadingContainerProps>) => {
  const [waiting, setWaiting] = useState(true)

  useEffect(() => {
    const id = setTimeout(() => setWaiting(false), delay)
    return () => clearTimeout(id)
  }, [delay])

  return !waiting && children
}
