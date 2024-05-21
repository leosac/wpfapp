using Leosac.WpfApp.Domain;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Leosac.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(MainSnackbar.MessageQueue!);
        }

        private void OnSelectedMenuItemChanged(object sender, DependencyPropertyChangedEventArgs e)
            => MainScrollViewer.ScrollToHome();

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen flag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;

            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar)
                {
                    dependencyObject = null;
                }
                else
                {
                    dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
                }
            }

            MenuToggleButton.IsChecked = false;
        }

        private void BtnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            MaintenancePlanHelper.OpenSubscription();
        }

        private void LinkRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MaintenancePlanHelper.OpenRegistration();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && DataContext is MainWindowViewModel model)
            {
                model.InitFromSettings();
            }

            LeosacWinAppInfo.WinInstance?.OnAppLoaded();
        }
    }
}
