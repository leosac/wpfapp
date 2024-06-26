﻿using Leosac.SharedServices;
using Leosac.WpfApp.Domain;
using System;
using System.Diagnostics;
using System.Windows;

namespace Leosac.WpfApp
{
    /// <summary>
    /// Interaction logic for PlanRegistrationWindow.xaml
    /// </summary>
    public partial class PlanRegistrationWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public PlanRegistrationWindow()
        {
            InitializeComponent();

            DataContext = new PlanRegistrationWindowViewModel();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnRegisterOffline_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PlanRegistrationWindowViewModel model && !string.IsNullOrEmpty(model.Key) && !string.IsNullOrEmpty(model.Code))
            {
                try
                {
                    var plan = MaintenancePlan.GetSingletonInstance();
                    plan.RegisterPlan(model.Key, null, model.Code);

                    this.DialogResult = true;
                }
                catch (Exception)
                {
                    model.LastError = "The offline registration failed.";
                }
            }
        }

        private void btnRegisterOnline_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PlanRegistrationWindowViewModel model && !string.IsNullOrEmpty(model.Key))
            {
                try
                {
                    var plan = MaintenancePlan.GetSingletonInstance();
                    plan.RegisterPlan(model.Key, model.Email);

                    this.DialogResult = true;
                }
                catch (Exception)
                {
                    model.LastError = "The online registration failed.";
                }
            }
        }

        private void OpenUrl_Click(object sender, RoutedEventArgs e)
        {
            OpenRegistrationUrl();
        }

        private void OpenRegistrationUrl()
        {
            if (DataContext is PlanRegistrationWindowViewModel model)
            {
                var ps = new ProcessStartInfo(model.OfflineRegistrationUrl)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            }
        }

        private void CopyUrl_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PlanRegistrationWindowViewModel model)
            {
                Clipboard.SetText(model.OfflineRegistrationUrl);
            }
        }
    }
}
