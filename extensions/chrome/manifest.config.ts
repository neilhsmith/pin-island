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
  minimum_chrome_version: "116",
  action: {
    default_popup: "src/pages/popup/index.html",
  },
  background: {
    service_worker: "src/service-workers/index.ts",
    type: "module",
  },
  content_scripts: [
    {
      js: ["src/content-scripts/open-side-panel/index.tsx"],
      matches: ["http://*/*", "https://*/*", "<all_urls>"],
    },
  ],
  options_page: "src/pages/options/index.html",
  side_panel: {
    default_path: "src/pages/side-panel/index.html",
  },
  permissions: ["contextMenus", "sidePanel", "identity", "storage"],
}))
