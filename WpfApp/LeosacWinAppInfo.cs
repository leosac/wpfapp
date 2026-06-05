using CommunityToolkit.Mvvm.Input;
using Leosac.SharedServices;
using Leosac.WpfApp.Domain;

namespace Leosac.WpfApp
{
    public abstract class LeosacWinAppInfo : LeosacAppInfo
    {
        private const string DefaultApplicationLogoPath = "/WpfApp;component/images/leosac.png";

        public static LeosacWinAppInfo? WinInstance => Instance as LeosacWinAppInfo;

        public string ApplicationLogo { get; protected set; }

        public RelayCommand? SettingsCommand { get; protected set; }

        protected LeosacWinAppInfo()
        {
            ApplicationLogo = DefaultApplicationLogoPath;
            SettingsCommand = null;
        }


        public abstract void InitializeMainWindow(MainWindowViewModel model);

        public abstract void InitializeAboutWindow(AboutWindowViewModel model);

        public virtual void OnAppLoaded()
        {
        }
    }
}
