import { MESSAGES } from "../constants"

const CONTEXT_MENU_ID = "openSidePanel"

chrome.runtime.onInstalled.addListener(() => {
  chrome.contextMenus.create({
    id: CONTEXT_MENU_ID,
    title: "Open Pin Island Side panel",
    contexts: ["all"],
  })
})

chrome.contextMenus.onClicked.addListener((info, tab) => {
  if (info.menuItemId === CONTEXT_MENU_ID) {
    chrome.sidePanel.open({ windowId: tab?.windowId })
  }
})

chrome.runtime.onMessage.addListener((message, sender) => {
  checkMessage()
  async function checkMessage() {
    if (message.type === MESSAGES.OPEN_SIDE_PANEL && !!sender.tab) {
      await chrome.sidePanel.open({ tabId: sender.tab.id })
      await chrome.sidePanel.setOptions({
        tabId: sender.tab.id,
        enabled: true,
      })
    }
  }

  return undefined
})
