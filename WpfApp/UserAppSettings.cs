using System;
using System.Diagnostics;
using System.Reflection;

namespace Leosac.WpfApp
{
    public class UserAppSettings : PermanentConfig<UserAppSettings>
    {
        private static readonly object _objlock = new();
        private static UserAppSettings? _singleton;

        public static UserAppSettings GetSingletonInstance()
        {
            return GetSingletonInstance(false);
        }

        public static UserAppSettings GetSingletonInstance(bool forceRecreate)
        {
            lock (_objlock)
            {
                if (_singleton == null || forceRecreate)
                {
                    _singleton = LoadFromFile(true);
                }

                return _singleton!;
            }
        }

        public UserAppSettings()
        {
            IsUserConfiguration = true;
        }

        private bool _useDarkTheme;
        public bool UseDarkTheme
        {
            get => _useDarkTheme;
            set => SetProperty(ref _useDarkTheme, value);
        }

        private string? _language;
        public string? Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }
    }
}
