using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PowerCenter.Win32Bridge.Services.PowerService
{
    internal static class PowerHelper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int SC_MONITORPOWER = 0xF170;
        private const int WM_SYSCOMMAND = 0x0112;

        public static void SendMessage(IntPtr hWnd, MonitorState state)
        {
            SendMessage(hWnd, WM_SYSCOMMAND, SC_MONITORPOWER, (int)state);
        }

        public static void Suspend()
        {
            Application.SetSuspendState(PowerState.Suspend, true, true);
        }
    }

    internal enum MonitorState
    {
        ON = -1,
        OFF = 2,
        STANDBY = 1
    }
}
