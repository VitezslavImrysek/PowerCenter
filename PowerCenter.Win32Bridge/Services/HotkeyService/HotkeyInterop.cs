using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Windows.System;

// Code from: https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp

namespace PowerCenter.Win32Bridge.Services.HotkeyService
{
    public sealed class HotkeyInterop
    {
        private List<HotkeyEntry> _hotkeys;
        private HotkeyHook _hotkeyHook;

        public HotkeyInterop()
        {
            _hotkeys = new List<HotkeyEntry>();

            _hotkeyHook = new HotkeyHook();
            // register the event that is fired after the key press.
            _hotkeyHook.KeyPressed += new EventHandler<HotkeyPressedEventArgs>(OnHotkeyPressed);
        }

        public event EventHandler<int> HotkeyPressed;

        public void Register(int id, Hotkey hotkey)
        {
            Keys key = (Keys)hotkey.Key;
            HotkeyModifierKeys modifiers = HotkeyModifierKeys.None;

            if ((hotkey.Modifiers & VirtualKeyModifiers.Control) == VirtualKeyModifiers.Control)
            {
                modifiers |= HotkeyModifierKeys.Control;
            }

            if ((hotkey.Modifiers & VirtualKeyModifiers.Menu) == VirtualKeyModifiers.Menu)
            {
                modifiers |= HotkeyModifierKeys.Alt;
            }

            if ((hotkey.Modifiers & VirtualKeyModifiers.Shift) == VirtualKeyModifiers.Shift)
            {
                modifiers |= HotkeyModifierKeys.Shift;
            }

            if ((hotkey.Modifiers & VirtualKeyModifiers.Windows) == VirtualKeyModifiers.Windows)
            {
                modifiers |= HotkeyModifierKeys.Win;
            }

            var nativeId = _hotkeyHook.RegisterHotKey(modifiers, key);
            _hotkeys.Add(new HotkeyEntry(id, nativeId, key, modifiers));
        }

        public void Unregister(int id)
        {
            var hotkeys = _hotkeys;
            for (int i = 0; i < hotkeys.Count; i++)
            {
                var entry = hotkeys[i];
                if (entry.Id == id)
                {
                    int nativeId = entry.NativeId;
                    _hotkeyHook.UnregisterHotKey(nativeId);

                    hotkeys.RemoveAt(i);
                    return;
                }
            }
        }

        private void OnHotkeyPressed(object sender, HotkeyPressedEventArgs e)
        {
            var key = e.Key;
            var modifiers = e.Modifier;

            var hotkeys = _hotkeys;
            for (int i = 0; i < hotkeys.Count; i++)
            {
                var entry = hotkeys[i];
                if (entry.Key == key && entry.Modifiers == modifiers)
                {
                    HotkeyPressed?.Invoke(this, entry.Id);
                    return;
                }
            }
        }

        private struct HotkeyEntry
        {
            public HotkeyEntry(int id, int nativeId, Keys key, HotkeyModifierKeys modifiers)
            {
                Id = id;
                NativeId = nativeId;
                Key = key;
                Modifiers = modifiers;
            }

            public int Id;
            public int NativeId;
            public Keys Key;
            public HotkeyModifierKeys Modifiers;
        }
    }
}
