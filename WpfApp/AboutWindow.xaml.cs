using Leosac.SharedServices;
using Leosac.WpfApp.Domain;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Leosac.WpfApp
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new AboutWindowViewModel();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void LinkWebsite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var ps = new ProcessStartInfo("https://leosac.com/")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AboutWindowViewModel model)
            {
                MaintenancePlanHelper.OpenRegistration();

                var plan = MaintenancePlan.GetSingletonInstance();
                model.IsActivePlan = plan.IsActivePlan();
                model.ExpirationDate = plan.ExpirationDate;
            }
        }

        private void BtnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            MaintenancePlanHelper.OpenSubscription();
        }

        private void BtnDownloadUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AboutWindowViewModel model)
            {
                model.AutoUpdate.DownloadUpdate();
            }
        }

        private void BtnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AboutWindowViewModel model)
            {
                model.AutoUpdate.CheckUpdate();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AutoUpdateToggleButton.IsChecked = true;
            if ((AppSettings.GetSingletonInstance()?.IsAutoUpdateEnabled).GetValueOrDefault(false))
            {
                CheckUpdate();
            }
        }

        private void AutoUpdateToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = AppSettings.GetSingletonInstance() ?? new AppSettings();
            settings.IsAutoUpdateEnabled = AutoUpdateToggleButton.IsChecked.GetValueOrDefault(false);
            if (settings.IsAutoUpdateEnabled)
            {
                CheckUpdate();
            }
            settings.SaveToFile();
        }

        private void CheckUpdate()
        {
            if (DataContext is AboutWindowViewModel model)
            {
                btnCheckUpdate.IsEnabled = false;
                model.AutoUpdate.CheckUpdate();
                btnCheckUpdate.IsEnabled = true;
            }
        }
    }
}
