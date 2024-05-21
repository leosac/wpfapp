using CommunityToolkit.Mvvm.Input;
using Leosac.SharedServices;
using Leosac.WpfApp.Domain;

namespace Leosac.WpfApp
{
    public abstract class LeosacWinAppInfo : LeosacAppInfo
    {
        public static LeosacWinAppInfo? WinInstance
        {
            get => Instance as LeosacWinAppInfo;
        }

        protected LeosacWinAppInfo()
        {
            ApplicationLogo = "/WpfApp;component/images/leosac.png";
            SettingsCommand = null;
        }

        public string ApplicationLogo { get; protected set; }

        public RelayCommand? SettingsCommand { get; protected set; }

        public abstract void InitializeMainWindow(MainWindowViewModel model);

        public abstract void InitializeAboutWindow(AboutWindowViewModel model);

        public virtual void OnAppLoaded()
        {
        }
    }
}
