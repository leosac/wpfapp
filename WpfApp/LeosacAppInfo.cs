using Leosac.WpfApp.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Leosac.WpfApp
{
    public abstract class LeosacAppInfo
    {
        public static LeosacAppInfo Instance { get; set; }

        public LeosacAppInfo()
        {
            var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
            ApplicationName = ApplicationTitle = fvi.ProductName ?? "Leosac Application";
            CheckPlan = true;
            PerUserInstallation = true;
            ApplicationUrl = "https://leosac.com";
            ApplicationLogo = "/WpfApp;component/images/leosac.png";
            SettingsCommand = null;
        }

        public string ApplicationName { get; protected set; }

        public string ApplicationTitle { get; protected set; }

        public string ApplicationCode { get; protected set; }

        public string ApplicationUrl { get; protected set; }

        public string ApplicationLogo { get; protected set; }

        public bool CheckPlan { get; protected set; }

        public bool PerUserInstallation { get; protected set; }

        public LeosacAppCommand? SettingsCommand { get; protected set; }

        public abstract void InitializeMainWindow(MainWindowViewModel model);

        public abstract void InitializeAboutWindow(AboutWindowViewModel model);
    }
}
