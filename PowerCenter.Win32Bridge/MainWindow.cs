using PowerCenter.Win32Bridge.Services.HotkeyService;
using PowerCenter.Win32Bridge.Services.PowerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;

namespace PowerCenter.Win32Bridge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Useful links
        // Global hotkeys: http://blog.magnusmontin.net/2015/03/31/implementing-global-hot-keys-in-wpf/
        // Win32 Bridge Interop - https://stefanwick.com/2017/05/26/uwp-calling-office-interop-apis/
        // WinRT API - https://www.thomasclaudiushuber.com/2019/04/26/calling-windows-10-apis-from-your-wpf-application/

        private const int HOTKEY_SLEEP_ID = 1;
        private const int HOTKEY_SLEEP_ALT_ID = 2;
        private const int HOTKEY_DISPLAY_OFF_ID = 3;

        private AppServiceConnection _connection = null;
        private HotkeyInterop _hotkeyInterop;
        private NotifyIcon _notifyIcon;
        private IntPtr _windowHandle;

        public MainWindow()
        {
            this.Hide();
            Initialize();
        }

        private async void Initialize()
        {
            WindowInteropHelper interopHelper = new WindowInteropHelper(this);
            _windowHandle = interopHelper.EnsureHandle();

            _hotkeyInterop = new HotkeyInterop();
            _hotkeyInterop.HotkeyPressed += OnHotkeyPressed;
            OnHotkeySettingsChanged();


            EnsureTaskBarIcon();
            await InitializeAppServiceConnection();
        }

        private async Task InitializeAppServiceConnection()
        {
            // https://github.com/microsoft/Windows-Packaging-Samples/blob/master/OfficeInterop/Excel.Interop/Program.cs
            var connection = new AppServiceConnection();
            connection.AppServiceName = ConnectionHelper.APPSERVICE_NAME;
            connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;

            AppServiceConnectionStatus status = await connection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                // Win32Brige app can be started on startup, in which case connection wont succeed.
                // TODO: error handling
                _connection = null;
            }
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var deferral = args.GetDeferral();
            try
            {
                // Determine which of those must be run on main thread?
                // Or do we run them all on the main thread to be on the safe side?
                var message = args.Request.Message;
                string action = (string)message["ACTION"];
                switch (action)
                {
                    case ConnectionHelper.VALUE_SLEEP:
                        OnSuspendRequested();
                        break;
                    case ConnectionHelper.VALUE_SLEEP_ALT:
                        OnMonitorStandbyRequested();
                        break;
                    case ConnectionHelper.VALUE_DISPLAY_OFF:
                        OnMonitorOffRequested();
                        break;
                    case ConnectionHelper.VALUE_HOTKEY_SETTINGS:
                        OnHotkeySettingsChanged();
                        break;
                    default:
                        break;
                }

                var result = new ValueSet();
                result.Add(ConnectionHelper.KEY_RESULT, ConnectionHelper.VALUE_OK);
                await args.Request.SendResponseAsync(result);
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                _connection?.Dispose();
                _connection = null;
            }));
        }

        private void EnsureTaskBarIcon()
        {
            var iconStream = System.Windows.Application.GetResourceStream(new Uri("/icon.ico", UriKind.Relative)).Stream;

            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Text = "PowerCenter";
            notifyIcon.DoubleClick += (s, e) => OnNotifyIconDoubleClicked();
            notifyIcon.Icon = new System.Drawing.Icon(iconStream);

            MenuItem item = new MenuItem();
            item.Text = "Exit";
            item.Click += (s, ev) => OnNotifyIconExitClicked();

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(item);

            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Visible = true;

            _notifyIcon = notifyIcon;
        }

        private void OnHotkeyPressed(object sender, int id)
        {
            switch (id)
            {
                case HOTKEY_SLEEP_ID:
                    OnSuspendRequested();
                    break;
                case HOTKEY_SLEEP_ALT_ID:
                    OnMonitorStandbyRequested();
                    break;
                case HOTKEY_DISPLAY_OFF_ID:
                    OnMonitorOffRequested();
                    break;
            }
        }

        private void OnSuspendRequested()
        {
            PowerHelper.Suspend();
        }

        private void OnMonitorStandbyRequested()
        {
            PowerHelper.SendMessage(_windowHandle, MonitorState.STANDBY);
        }

        private void OnMonitorOffRequested()
        {
            PowerHelper.SendMessage(_windowHandle, MonitorState.OFF);
        }

        private void OnHotkeySettingsChanged()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                // Following code must be run on thread which created HotkeyInterop->HotkeyHook->NativeWindow.Handle.
                HotkeyInterop hotkeyInterop = _hotkeyInterop;
                hotkeyInterop.Unregister(HOTKEY_SLEEP_ID);
                hotkeyInterop.Unregister(HOTKEY_SLEEP_ALT_ID);
                hotkeyInterop.Unregister(HOTKEY_DISPLAY_OFF_ID);

                if (SettingsHelper.IsHotkeysEnabled)
                {
                    Hotkey sleepHotkey = SettingsHelper.SleepHotkey;
                    if (sleepHotkey.IsSet)
                    {
                        hotkeyInterop.Register(HOTKEY_SLEEP_ID, sleepHotkey);
                    }

                    Hotkey sleepAlternativeHotkey = SettingsHelper.SleepAlternativeHotkey;
                    if (sleepAlternativeHotkey.IsSet)
                    {
                        hotkeyInterop.Register(HOTKEY_SLEEP_ALT_ID, sleepAlternativeHotkey);
                    }

                    Hotkey displayOffHotkey = SettingsHelper.DisplayOffHotkey;
                    if (displayOffHotkey.IsSet)
                    {
                        hotkeyInterop.Register(HOTKEY_DISPLAY_OFF_ID, displayOffHotkey);
                    }
                }
            }));
        }

        private async void OnNotifyIconDoubleClicked()
        {
            IEnumerable<AppListEntry> appListEntries = await Package.Current.GetAppListEntriesAsync();
            await appListEntries.First().LaunchAsync();

            if (_connection == null)
            {
                await InitializeAppServiceConnection();
            }
        }

        private async void OnNotifyIconExitClicked()
        {
            if (_connection != null)
            {
                // UWP UI App is opened - request exit.
                var request = new ValueSet();
                request.Add(ConnectionHelper.KEY_ACTION, ConnectionHelper.VALUE_EXIT);

                await _connection.SendMessageAsync(request);
            }

            // Dispose of tray icon.
            _notifyIcon.Dispose();

            // Exit WPF application.
            System.Windows.Application.Current.Shutdown();
        }
    }
}
