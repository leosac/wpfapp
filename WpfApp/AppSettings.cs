using System;
using System.Diagnostics;
using System.Reflection;

namespace Leosac.WpfApp
{
    public class AppSettings : PermanentConfig<AppSettings>
    {
        private static readonly object _objlock = new();
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
            _installationId = Guid.NewGuid().ToString("D");
            _isAutoUpdateEnabled = true;
        }

        private string _installationId;
        public string InstallationId
        {
            get => _installationId;
            set => SetProperty(ref _installationId, value);
        }

        private bool _useDarkTheme;
        public bool UseDarkTheme
        {
            get => _useDarkTheme;
            set => SetProperty(ref _useDarkTheme, value);
        }

        private bool _isAutoUpdateEnabled;
        public bool IsAutoUpdateEnabled
        {
            get => _isAutoUpdateEnabled;
            set => SetProperty(ref _isAutoUpdateEnabled, value);
        }

        private string? _language;
        public string? Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }

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
