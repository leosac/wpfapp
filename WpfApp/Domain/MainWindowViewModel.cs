using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Leosac.SharedServices;

namespace Leosac.WpfApp.Domain
{
    public class MainWindowViewModel : ObservableObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            log.Debug("Initializing KeyManager MainWindow view model...");

            LogConsoleCommand = new RelayCommand(
                () =>
                {
                    var consoleWindow = new LogConsoleWindow();
                    consoleWindow.Show();
                });
            OpenAboutCommand = new RelayCommand(
                () =>
                {
                    var aboutWindow = new AboutWindow();
                    aboutWindow.ShowDialog();
                });
            ChangeLanguageCommand = new RelayCommand<string?>(
                lang =>
                {
                    if (string.IsNullOrEmpty(lang))
                    {
                        lang = "en-US";
                    }

                    var settings = UserAppSettings.GetSingletonInstance();
                    settings.Language = lang;
                    settings.SaveToFile();

                    LangHelper.ChangeLanguage(lang);

                    // Restart the application to be sure already created Windows have expected language
                    var module = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                    if (module != null && !string.IsNullOrEmpty(module.FileName))
                    {
                        System.Diagnostics.Process.Start(module.FileName);
                    }
                    Application.Current.Shutdown();
                });

            SnackbarMessageQueue = snackbarMessageQueue;
            MenuItems = new ObservableCollection<NavItem>();
            LeosacWinAppInfo.WinInstance?.InitializeMainWindow(this);
            if (MenuItems.Count > 0)
            {
                SelectedItem = MenuItems[0];
                SelectedIndex = 0;
            }
            if (LeosacAppInfo.Instance != null && LeosacAppInfo.Instance.CheckPlan)
            {
                var plan = MaintenancePlan.GetSingletonInstance();
                _showPlanFooter = !plan.IsActivePlan();
                plan.PlanUpdated += (sender, e) => { ShowPlanFooter = !plan.IsActivePlan(); };
            }
            else
            {
                _showPlanFooter = false;
            }
        }

        private NavItem? _selectedItem;
        private int _selectedIndex;
        private bool _showPlanFooter;
        private bool _isDarkMode;

        public ObservableCollection<NavItem> MenuItems { get; }

        public ISnackbarMessageQueue SnackbarMessageQueue { get; }

        public NavItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public bool ShowPlanFooter
        {
            get => _showPlanFooter;
            set => SetProperty(ref _showPlanFooter, value);
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                SetProperty(ref _isDarkMode, value);
                ModifyAndSaveTheme(value);
            }
        }

        public RelayCommand LogConsoleCommand { get; }
        public RelayCommand OpenAboutCommand { get; }
        public RelayCommand<string?> ChangeLanguageCommand { get; }

        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            if (isDarkTheme)
            {
                theme.SetDarkTheme();
            }
            else
            {
                theme.SetLightTheme();
            }
            paletteHelper.SetTheme(theme);
        }

        public static void ModifyAndSaveTheme(bool isDarkTheme)
        {
            ModifyTheme(isDarkTheme);

            var settings = UserAppSettings.GetSingletonInstance();
            settings.UseDarkTheme = isDarkTheme;
            settings.SaveToFile();
        }

        public void InitFromSettings()
        {
            var userSettings = UserAppSettings.GetSingletonInstance();
            if (userSettings.UseDarkTheme)
            {
                IsDarkMode = userSettings.UseDarkTheme;
            }
            var settings = AppSettings.GetSingletonInstance();
            if (settings.IsAutoUpdateEnabled)
            {
                Application.Current.Dispatcher.BeginInvoke(async () =>
                {
                    var update = new AutoUpdate();
                    if (await update.CheckUpdate())
                    {
                        var wrapControl = new WrapPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(5),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        var textControl = new TextBlock
                        {
                            Text = Properties.Resources.NewUpdateAvailable
                        };
                        wrapControl.Children.Add(textControl);
                        var buttonControl = new Button
                        {
                            Content = Properties.Resources.DownloadNow
                        };
                        buttonControl.Click += (sender, e) => { update.DownloadUpdate(); };
                        buttonControl.Style = Application.Current.FindResource("MaterialDesignFlatButton") as Style;
                        buttonControl.Margin = new Thickness(20, 0, 0, 0);
                        wrapControl.Children.Add(buttonControl);

                        SnackbarMessageQueue.Enqueue(wrapControl, null, null, null, false, true, TimeSpan.FromSeconds(5));
                    }
                });
            }
        }
    }
}
