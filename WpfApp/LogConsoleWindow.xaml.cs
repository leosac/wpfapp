using Leosac.WpfApp.Domain;
using System.Windows;
using System.Windows.Controls;

namespace Leosac.WpfApp
{
    /// <summary>
    /// Interaction logic for LogConsoleWindow.xaml
    /// </summary>
    public partial class LogConsoleWindow : Window
    {
        public LogConsoleWindow()
        {
            InitializeComponent();

            DataContext = new LogConsoleWindowViewModel();
        }

        private void TbxLogOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbxLogOutput.ScrollToEnd();
        }
    }
}
