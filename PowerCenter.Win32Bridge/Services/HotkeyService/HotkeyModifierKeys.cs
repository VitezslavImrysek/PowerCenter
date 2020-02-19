using System;

// Code from: https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp

namespace PowerCenter.Win32Bridge.Services.HotkeyService
{
    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum HotkeyModifierKeys : uint
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
