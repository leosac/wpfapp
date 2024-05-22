using Leosac.SharedServices;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace Leosac.WpfApp
{
    public static class UserElevationHelper
    {
        public static bool MayRequireElevation<T>(this PermanentConfig<T> config) where T : PermanentConfig<T>, new()
        {
            return (!config.IsUserConfiguration && !PermanentConfig<T>.IsPerUserRunningApplication() && !IsAdministrator());
        }

        private static bool IsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static bool EnsureElevation<T>(this PermanentConfig<T> config) where T : PermanentConfig<T>, new()
        {
            if (MayRequireElevation(config))
            {
                return !Elevate();
            }

            return true;
        }

        public static bool Elevate()
        {
            if (!string.IsNullOrEmpty(Environment.ProcessPath) && MessageBox.Show("Additional permissions may be required. Would you like to restart the application with administrator privileges?", "User Elevation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var psi = new ProcessStartInfo(Environment.ProcessPath, Environment.GetCommandLineArgs());
                psi.UseShellExecute = true;
                psi.Verb = "runas";
                Process.Start(psi);
                Application.Current.Shutdown();
                return true;
            }

            return false;
        }
    }
}
