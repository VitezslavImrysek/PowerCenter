using System.Threading;
using System.Windows;
using Windows.ApplicationModel;

namespace PowerCenter.Win32Bridge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _myMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            _myMutex = new Mutex(true, Package.Current.Id.FamilyName, out bool isNewInstance);
            if (!isNewInstance)
            {
                App.Current.Shutdown();
            }
            else
            {
                MainWindow = new MainWindow();
                base.OnStartup(e);
            }
        }
    }
}
