using System;
using System.Reflection;

namespace PowerCenter.UWP.ViewModels
{
    public sealed class SettingsViewModel : ViewModelBase
    {
        private bool _isStartupEnabled;
        private bool _isHotkeysEnabled;

        private Hotkey _sleepHotkey;
        private Hotkey _sleepAlternativeHotkey;
        private Hotkey _displayOffHotkey;

        public SettingsViewModel()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionString = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

            Initialize();
        }

        public event EventHandler Win32BridgeSettingsChanged;

        public string VersionString { get; }

        public bool IsStartupEnabled
        {
            get { return _isStartupEnabled; }
            set { SetIsStartup(value); }
        }

        public bool IsHotkeysEnabled
        {
            get { return _isHotkeysEnabled; }
            set
            {
                SettingsHelper.IsHotkeysEnabled = value;
                SetProperty(ref _isHotkeysEnabled, value);
                RaiseWin32BridgeSettingsChanged();
            }
        }

        public Hotkey SleepHotkey
        {
            get { return _sleepHotkey; }
            set
            {
                SettingsHelper.SleepHotkey = value;
                SetProperty(ref _sleepHotkey, value);
                RaiseWin32BridgeSettingsChanged();
            }
        }

        public Hotkey SleepAlternativeHotkey
        {
            get { return _sleepAlternativeHotkey; }
            set
            {
                SettingsHelper.SleepAlternativeHotkey = value;
                SetProperty(ref _sleepAlternativeHotkey, value);
                RaiseWin32BridgeSettingsChanged();
            }
        }

        public Hotkey DisplayOffHotkey
        {
            get { return _displayOffHotkey; }
            set
            {
                SettingsHelper.DisplayOffHotkey = value;
                SetProperty(ref _displayOffHotkey, value);
                RaiseWin32BridgeSettingsChanged();
            }
        }

        private async void Initialize()
        {
            _isStartupEnabled = await StartupHelper.GetIsEnabled();
            _isHotkeysEnabled = SettingsHelper.IsHotkeysEnabled;

            _sleepHotkey = SettingsHelper.SleepHotkey;
            _sleepAlternativeHotkey = SettingsHelper.SleepAlternativeHotkey;
            _displayOffHotkey = SettingsHelper.DisplayOffHotkey;
        }

        private async void SetIsStartup(bool value)
        {
            SetProperty(ref _isStartupEnabled, await StartupHelper.TrySetIsEnabled(value));
        }

        private void RaiseWin32BridgeSettingsChanged()
        {
            Win32BridgeSettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
