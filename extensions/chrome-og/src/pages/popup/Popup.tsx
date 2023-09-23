import withSuspense from "@src/shared/hoc/withSuspense"

const Popup = () => {
  return (
    <div className="absolute inset-0 h-full p-10 text-center bg-slate-800">
      <p className="text-4xl text-white">sup bitch</p>
    </div>
  )
}

export default withSuspense(Popup)
