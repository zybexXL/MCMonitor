# MC Monitor

MC Monitor listens to events from JRiver's Media Center and performs actions in response.

The events typically report when a file starts playing, is paused or stopped, when the volume changes, etc. 
MC MOnitor must be run on the same PC as Media Center as it connected via OLE Automation - MCWS doe not support events.

Currently MCMonitor can:
- Write the event info into a JSON file containing information for current and next files
- Write the event info to a WebSocket
- Write an event log
- Execute custom commands

# Instructions

- Drop MCMonitor.exe in any folder with Write permissions
- Run it once - Notepad will open the default configuration, edit to your needs
- Run it again to start. It will stay running as a System Tray icon
- Click the tray icon to open a minimal status panel

The default configuration file has many comments explaining what each option does.
