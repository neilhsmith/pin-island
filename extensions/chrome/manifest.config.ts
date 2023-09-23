import { defineManifest } from "@crxjs/vite-plugin"
import packageJson from "./package.json"

const { version } = packageJson
const [major, minor, patch, label = "0"] = version
  .replace(/[^\d.-]+/g, "") // can only contain digits, dots, or dash
  .split(/[.-]/) // split into version parts

export default defineManifest(async () => ({
  manifest_version: 3,
  name: packageJson.name,
  description: packageJson.description,
  version: `${major}.${minor}.${patch}.${label}`,
  version_name: version,
  action: {
    default_popup: "src/apps/popup/index.html",
  },
  content_scripts: [
    {
      js: ["src/apps/sidebar/index.tsx"],
      matches: ["http://*/*", "https://*/*", "<all_urls>"],
    },
  ],
}))
