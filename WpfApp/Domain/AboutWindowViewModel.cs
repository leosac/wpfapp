using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace Leosac.WpfApp.Domain
{
    public class AboutWindowViewModel : ObservableObject
    {
        public class Library
        {
            public Library(string name, string license, string? description = null, string? url = null)
            {
                Name = name;
                License = license;
                Description = description;
                Url = url;
            }

            public string Name { get; private set; }

            public string License { get; private set; }

            public string? Description { get; private set; }

            public string? Url { get; private set; }
        }

        public AboutWindowViewModel()
        {
            GetInfoFromAssembly();

            _settings = AppSettings.GetSingletonInstance();
            _autoUpdate = new AutoUpdate();

            Libraries = new ObservableCollection<Library>(new[]
            {
                new Library("Microsoft.NETCore.App", "MIT", ".NET Core SDK", "https://github.com/dotnet/runtime"),
                new Library("Microsoft.WindowsDesktop.App.WPF", "MIT", ".NET Core UI Framework", "https://github.com/dotnet/wpf"),
                new Library("log4net", "Apache v2", "Logging library", "https://logging.apache.org/log4net/"),
                new Library("MaterialDesignInXaml", "MIT", "Graphic library", "http://materialdesigninxaml.net/"),
                new Library("Json.NET", "MIT", "JSON library", "https://www.newtonsoft.com/json"),
                new Library("CommunityToolkit.Mvvm", "MIT", "MVVM library", "https://github.com/CommunityToolkit/dotnet")
            });

            _showSourceCodeLicense = true;
            LeosacAppInfo.Instance.InitializeAboutWindow(this);

            var plan = MaintenancePlan.GetSingletonInstance();
            _isActivePlan = plan.IsActivePlan();
            _expirationDate = plan.ExpirationDate;
        }

        private string? _softwareName;
        private string? _softwareVersion;
        private AppSettings _settings;
        private AutoUpdate _autoUpdate;
        private bool _isActivePlan;
        private DateTime? _expirationDate;
        private bool _showSourceCodeLicense;

        public string? SoftwareName
        {
            get => _softwareName;
            set => SetProperty(ref _softwareName, value);
        }

        public string? SoftwareVersion
        {
            get => _softwareVersion;
            set => SetProperty(ref _softwareVersion, value);
        }

        public AppSettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public AutoUpdate AutoUpdate
        {
            get => _autoUpdate;
            set => SetProperty(ref _autoUpdate, value);
        }

        public bool IsActivePlan
        {
            get => _isActivePlan;
            set => SetProperty(ref _isActivePlan, value);
        }

        public DateTime? ExpirationDate
        {
            get => _expirationDate;
            set => SetProperty(ref _expirationDate, value);
        }

        public bool ShowSourceCodeLicense
        {
            get => _showSourceCodeLicense;
            set => SetProperty(ref _showSourceCodeLicense, value);
        }

        public ObservableCollection<Library> Libraries { get; }

        private void GetInfoFromAssembly()
        {
            var fvi = AppSettings.GetFileVersionInfo();
            _softwareName = fvi?.ProductName;
            _softwareVersion = fvi?.ProductVersion;
        }
    }
}
