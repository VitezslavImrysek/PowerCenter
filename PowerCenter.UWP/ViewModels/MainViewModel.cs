using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace PowerCenter.UWP.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        private Command _sleepCommand;
        private Command _sleepAltCommand;
        private Command _displayOffCommand;

        public MainViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
            SettingsViewModel.Win32BridgeSettingsChanged += OnWin32BridgeSettingsChanged;
        }

        public SettingsViewModel SettingsViewModel { get; }

        public Command SleepCommand => _sleepCommand ?? (_sleepCommand = new Command(Sleep));
        public Command SleepAltCommand => _sleepAltCommand ?? (_sleepAltCommand = new Command(SleepAlt));
        public Command DisplayOffCommand => _displayOffCommand ?? (_displayOffCommand = new Command(DisplayOff));

        public async void OnNavigatedTo()
        {
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                App.AppServiceConnected += App_AppServiceConnected;
                App.AppServiceDisconnected += MainPage_AppServiceDisconnected;
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        private async void Sleep()
        {
            ValueSet request = new ValueSet();
            request.Add(ConnectionHelper.KEY_ACTION, ConnectionHelper.VALUE_SLEEP);
            AppServiceResponse response = await App.Connection.SendMessageAsync(request);
        }

        private async void SleepAlt()
        {
            ValueSet request = new ValueSet();
            request.Add(ConnectionHelper.KEY_ACTION, ConnectionHelper.VALUE_SLEEP_ALT);
            AppServiceResponse response = await App.Connection.SendMessageAsync(request);
        }

        private async void DisplayOff()
        {
            ValueSet request = new ValueSet();
            request.Add(ConnectionHelper.KEY_ACTION, ConnectionHelper.VALUE_DISPLAY_OFF);
            AppServiceResponse response = await App.Connection.SendMessageAsync(request);
        }

        private async void RefreshWin32HotkeysSettings()
        {
            ValueSet request = new ValueSet();
            request.Add(ConnectionHelper.KEY_ACTION, ConnectionHelper.VALUE_HOTKEY_SETTINGS);
            AppServiceResponse response = await App.Connection.SendMessageAsync(request);
        }

        private void OnWin32BridgeSettingsChanged(object sender, EventArgs e)
        {
            RefreshWin32HotkeysSettings();
        }

        private async void App_AppServiceConnected(object sender, AppServiceTriggerDetails e)
        {
            App.Connection.RequestReceived += AppServiceConnection_RequestReceived;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // Connected
            });
        }

        private async void MainPage_AppServiceDisconnected(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // Disconnected

                Reconnect();
            });
        }

        private async void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var deferral = args.GetDeferral();

            var message = args.Request.Message;
            var action = (string)message["ACTION"];

            ValueSet response = new ValueSet();
            response.Add("RESULT", "OK");
            await args.Request.SendResponseAsync(response);

            deferral.Complete();

            if (action == "EXIT")
            {
                Application.Current.Exit();
            }
        }

        private async void Reconnect()
        {
            if (App.IsForeground)
            {
                MessageDialog dlg = new MessageDialog("Connection to desktop process lost. Reconnect?");
                UICommand yesCommand = new UICommand("Yes", async (r) =>
                {
                    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                });
                UICommand noCommand = new UICommand("No", (r) =>
                {

                });
                dlg.Commands.Add(yesCommand);
                dlg.Commands.Add(noCommand);

                await dlg.ShowAsync();
            }
        }
    }
}
