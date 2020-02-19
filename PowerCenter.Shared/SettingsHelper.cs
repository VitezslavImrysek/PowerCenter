using System.Runtime.CompilerServices;
using Windows.Storage;

namespace PowerCenter
{
    public static class SettingsHelper
    {
        private static ApplicationDataContainer SettingsStorage;

        static SettingsHelper()
        {
            SettingsStorage = ApplicationData.Current.LocalSettings;
        }

        public static bool IsHotkeysEnabled
        {
            get => GetValue<bool>();
            set => SetValue<bool>(value);
        }

        public static Hotkey SleepHotkey
        {
            get => GetHotkey();
            set => SetHotkey(value);
        }

        public static Hotkey SleepAlternativeHotkey
        {
            get => GetHotkey();
            set => SetHotkey(value);
        }

        public static Hotkey DisplayOffHotkey
        {
            get => GetHotkey();
            set => SetHotkey(value);
        }

        private static T GetValue<T>([CallerMemberName]string settingsKey = null)
        {
            T value = default(T);
            if (SettingsStorage.Values.TryGetValue(settingsKey, out object keyValue))
            {
                value = (T)keyValue;
            }

            return value;
        }

        private static void SetValue<T>(T value, [CallerMemberName]string settingsKey = null)
        {
            SettingsStorage.Values[settingsKey] = value;
        }

        private static Hotkey GetHotkey([CallerMemberName]string settingsKey = null)
        {
            return Hotkey.DeserializeHotkey(GetValue<string>(settingsKey));
        }

        private static void SetHotkey(Hotkey hotkey, [CallerMemberName]string settingsKey = null)
        {
            if (!hotkey.IsSet)
            {
                hotkey = new Hotkey();
            }

            SetValue<string>(Hotkey.SerializeHotkey(hotkey), settingsKey);
        }
    }
}
