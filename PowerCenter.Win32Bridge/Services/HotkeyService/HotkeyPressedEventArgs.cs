using System;
using System.Windows.Forms;

// Code from: https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp

namespace PowerCenter.Win32Bridge.Services.HotkeyService
{
    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class HotkeyPressedEventArgs : EventArgs
    {
        private HotkeyModifierKeys _modifier;
        private Keys _key;

        internal HotkeyPressedEventArgs(HotkeyModifierKeys modifier, Keys key)
        {
            _modifier = modifier;
            _key = key;
        }

        public HotkeyModifierKeys Modifier
        {
            get { return _modifier; }
        }

        public Keys Key
        {
            get { return _key; }
        }
    }
}
