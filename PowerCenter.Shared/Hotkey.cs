using System;
using System.Text;
using Windows.System;

namespace PowerCenter
{
    public struct Hotkey
    {
        public Hotkey(VirtualKey key, VirtualKeyModifiers modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public VirtualKey Key { get; }
        public VirtualKeyModifiers Modifiers { get; }

        public bool IsSet => 
            Key != VirtualKey.None 
            && Modifiers != VirtualKeyModifiers.None;

        public static string SerializeHotkey(Hotkey hotkey)
        {
            var key = hotkey.Key;
            var modifiers = hotkey.Modifiers;

            StringBuilder sb = new StringBuilder();
            if ((modifiers & VirtualKeyModifiers.Control) == VirtualKeyModifiers.Control)
            {
                sb.Append($"{nameof(VirtualKeyModifiers.Control)}+");
            }
            if ((modifiers & VirtualKeyModifiers.Menu) == VirtualKeyModifiers.Menu)
            {
                sb.Append($"{nameof(VirtualKeyModifiers.Menu)}+");
            }
            if ((modifiers & VirtualKeyModifiers.Windows) == VirtualKeyModifiers.Windows)
            {
                sb.Append($"{nameof(VirtualKeyModifiers.Windows)}+");
            }
            if ((modifiers & VirtualKeyModifiers.Shift) == VirtualKeyModifiers.Shift)
            {
                sb.Append($"{nameof(VirtualKeyModifiers.Shift)}+");
            }

            sb.Append(key.ToString());

            return sb.ToString();
        }

        public static Hotkey DeserializeHotkey(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new Hotkey();
            }

            var key = VirtualKey.None;
            var modifiers = VirtualKeyModifiers.None;

            var textParts = text.Split('+');
            for (int i = 0; i < textParts.Length - 1; i++)
            {
                modifiers |= (VirtualKeyModifiers)Enum.Parse(typeof(VirtualKeyModifiers), textParts[i]);
            }
            key = (VirtualKey)Enum.Parse(typeof(VirtualKey), textParts[textParts.Length - 1]);

            return new Hotkey(key, modifiers);
        }
    }
}
