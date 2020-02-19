# PowerCenter
This project is a demonstration of UWP-Win32 bridge implementation.

# Features
Provides quick access to power-related operations like "Sleep" and "Turn Display Off".
These actions are accessible either through the application UI or through user-customizable global hotkeys.
Application supports automatic startup after user login.

# Architecture
Result is packaged as MSIX. MSIX composes of two parts:<br />
- UWP part (PowerCenter.UWP): Contains UI.<br />
- Win32 part (PowerCenter.Win32Bridge): Provides power and hotkey services through calls to Win32 API.<br />

These two parts share some common logic (PowerCenter.Shared) related to accessing user settings (hotkeys, startup) 
and sending action commands between the two parts.
Communication itself is implemented through AppServiceConnection WinRT API.

🐱‍🏍
