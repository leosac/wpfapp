using CommunityToolkit.Mvvm.Input;
using Leosac.WpfApp;
using Leosac.WpfApp.Domain;
using WpfAppDemo.Domain;

namespace WpfAppDemo
{
    public class DemoAppInfo : LeosacWinAppInfo
    {
        public DemoAppInfo()
        {
            ApplicationName = "Demo App";
            ApplicationTitle = "Leosac Wpf Demo App";
        }

        public override void InitializeAboutWindow(AboutWindowViewModel model)
        {
            
        }

        public override void InitializeMainWindow(MainWindowViewModel model)
        {
            var HomeCommand = new RelayCommand(
                () =>
                {
                    model.SelectedIndex = 0;
                });

            model.MenuItems.Add(new NavItem(
                "Home",
                typeof(HomeControl),
                "House",
                new HomeControlViewModel(model.SnackbarMessageQueue)
            ));
        }
    }
}
