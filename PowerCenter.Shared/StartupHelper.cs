using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace PowerCenter
{
    internal static class StartupHelper
    {
        public static async Task<bool> GetIsEnabled()
        {
            var startupTask = await StartupTask.GetAsync("PowerCenterTask");
            return startupTask.State == StartupTaskState.Enabled;
        }

        public static async Task<bool> TrySetIsEnabled(bool value)
        {
            var startupTask = await StartupTask.GetAsync("PowerCenterTask");
            if (value)
            {
                // Enable
                var state = await startupTask.RequestEnableAsync();
                return state == StartupTaskState.Enabled;
            }
            else
            {
                startupTask.Disable();
                return false;
            }
        }
    }
}
