﻿using Leosac.SharedServices;
using System.Diagnostics;

namespace Leosac.WpfApp
{
    public static class MaintenancePlanHelper
    {
        public static void OpenSubscription()
        {
            if (!string.IsNullOrEmpty(LeosacWinAppInfo.Instance?.ApplicationUrl))
            {
                var ps = new ProcessStartInfo(LeosacWinAppInfo.Instance.ApplicationUrl)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            }
        }

        public static void OpenRegistration()
        {
            var plan = MaintenancePlan.GetSingletonInstance();
            if (plan.EnsureElevation())
            {
                var dialog = new PlanRegistrationWindow();
                dialog.ShowDialog();
            }
        }
    }
}
