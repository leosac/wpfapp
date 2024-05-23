using Leosac.SharedServices;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfAppDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            LeosacAppInfo.Instance = new DemoAppInfo();
        }
    }
}
