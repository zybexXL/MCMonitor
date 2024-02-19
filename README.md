# MC Monitor

MC Monitor listens to events from JRiver's Media Center and performs actions in response.

The events typically report when a file starts playing, is paused or stopped, when the volume changes, etc. 
MC MOnitor must be run on the same PC as Media Center as it connected via OLE Automation - MCWS doe not support events.

Currently MCMonitor can:
- Write the event info into a JSON file containing information for current and next files
- Write the event info to a WebSocket
- Write an event log
- Execute custom commands

![MCMonitor status panel](/Screenshots/MCMonitor10.png)

# Instructions
- Install Net8 runtime if you don't have it yet:
> C:\\> winget install Microsoft.DotNet.DesktopRuntime.8
- Get MCMonitor [latest release](https://github.com/zybexXL/MCMonitor/releases)
- Extract to any folder with Write prmissions
- Run it once - Notepad will open the default configuration, edit to your needs
- Run it again to start. It will stay running as a System Tray icon
- Click the tray icon to open a minimal status panel

The default configuration file has many comments explaining what each option does.
You can see it here: https://github.com/zybexXL/MCMonitor/blob/main/Config/config.ini
