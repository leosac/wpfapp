using Leosac.WpfApp.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Leosac.WpfApp
{
    public class AppSettings : PermanentConfig<AppSettings>
    {
        public static string DefaultFileName { get => "AppSettings.json"; }

        private static object _objlock = new object();
        private static AppSettings? _singleton;

        public static AppSettings GetSingletonInstance(bool forceRecreate = false)
        {
            lock (_objlock)
            {
                if (_singleton == null || forceRecreate)
                {
                    _singleton = LoadFromFile();
                }

                return _singleton!;
            }
        }

        public AppSettings()
        {
            InstallationId = Guid.NewGuid().ToString("D");
            UseDarkTheme = false;
            IsAutoUpdateEnabled = true;
        }

        public string InstallationId { get; set; }

        public bool UseDarkTheme { get; set; }

        public bool IsAutoUpdateEnabled { get; set; }

        public string? Language { get; set; }

        public static FileVersionInfo? GetFileVersionInfo()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                return FileVersionInfo.GetVersionInfo(assembly.Location);
            }

            return null;
        }
    }
}
