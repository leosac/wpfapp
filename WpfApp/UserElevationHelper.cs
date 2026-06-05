using Leosac.SharedServices;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows;

namespace Leosac.WpfApp
{
    public static class UserElevationHelper
    {
        private const int UserCancelledErrorCode = 1223;

        public static bool MayRequireElevation<T>(this PermanentConfig<T> config) where T : PermanentConfig<T>, new()
        {
            return (!config.IsUserConfiguration && !PermanentConfig<T>.IsPerUserRunningApplication() && !IsAdministrator());
        }

        private static bool IsAdministrator()
        {
            try
            {
                using (var identity = WindowsIdentity.GetCurrent())
                {
                    var principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
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
            if (string.IsNullOrEmpty(Environment.ProcessPath))
                return false;
            if (MessageBox.Show(Properties.Resources.UserElevation, Properties.Resources.UserElevationTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;
            try
            {
                var psi = new ProcessStartInfo(Environment.ProcessPath)
                {
                    Arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1)),
                    UseShellExecute = true,
                    Verb = "runas"
                };
                var process = Process.Start(psi);
                if (process is null)
                    return false;
                Application.Current.Shutdown();
                return true;
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == UserCancelledErrorCode)
            {
                return false;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
