﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Leosac.WpfApp
{
    /// <summary>
    /// Maintenance plan.
    /// </summary>
    /// <remarks>
    /// This is not a security feature neither a proper licensing feature. A generator could easily be created.
    /// It is mainly designed to help Leosac company to manage the commercial plans provided to customers and the appropriate registered support services.
    /// It is also a way to drive end-users making profits with this software to commercialisation, for their own benefits (support of the software editor, maintenance support, ...).
    /// </remarks>
    public class MaintenancePlan : PermanentConfig<MaintenancePlan>
    {
        const string BASE_URL = "https://leosac.com/";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private static readonly object _objlock = new();
        private static MaintenancePlan? _singleton;

        public static MaintenancePlan GetSingletonInstance()
        {
            return GetSingletonInstance(false);
        }

        public static MaintenancePlan GetSingletonInstance(bool forceRecreate)
        {
            lock (_objlock)
            {
                if (_singleton == null || forceRecreate)
                {
                    try
                    {
                        _singleton = LoadFromFile();
                        _singleton?.ParseCode(_singleton.Code);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Maintenance Plan initialization failed.", ex);
                    }
                }

                return _singleton!;
            }
        }

        public MaintenancePlan()
        {

        }

        public string? LicenseKey { get; set; }

        [JsonIgnore]
        public DateTime? ExpirationDate { get; set; }

        public string? Code { get; set; }

        [JsonIgnore]
        public EventHandler? PlanUpdated { get; set; }

        public void OnPlanUpdated()
        {
            PlanUpdated?.Invoke(this, new EventArgs());
        }

        public override void SaveToFile()
        {
            base.SaveToFile();
            OnPlanUpdated();
        }

        public bool IsActivePlan()
        {
            if (!string.IsNullOrEmpty(LicenseKey))
            {
                return (ExpirationDate == null || ExpirationDate.Value >= DateTime.Now);
            }
            return false;
        }

        private static JToken? QueryData(string url)
        {
            return QueryData(url, null);
        }

        private static JToken? QueryData(string url, string[]? ignoreErrorCodes)
        {
            var client = new HttpClient();
            var req = client.GetStringAsync(url);
            var result = req.Result;
            if (string.IsNullOrEmpty(result))
            {
                var error = "The request failed without details.";
                log.Error(error);
                throw new MaintenanceException(error);
            }

            var jobj = JObject.Parse(result);
            var success = (bool?)jobj["success"];
            if (!success.GetValueOrDefault(false))
            {
                var errorCode = (string?)jobj["data"]?["code"];
                if (ignoreErrorCodes != null && !string.IsNullOrEmpty(errorCode) && ignoreErrorCodes.Contains(errorCode))
                {
                    log.Info(string.Format("Error code `{0}` ignored.", errorCode));
                }
                else
                {
                    var error = string.Format("The request failed with error: {0}", (string?)jobj["data"]?["error"] ?? (string?)jobj["data"]?["message"]);
                    log.Error(error);
                    throw new MaintenanceException(error);
                }
            }

            return jobj["data"];
        }

        public void RegisterPlan(string licenseKey)
        {
            RegisterPlan(licenseKey, null, null);
        }

        public void RegisterPlan(string licenseKey, string? email)
        {
            RegisterPlan(licenseKey, email, null);
        }

        public void RegisterPlan(string licenseKey, string? email, string? code)
        {
            if (string.IsNullOrEmpty(licenseKey))
            {
                throw new MaintenanceException("License/Plan key is required.");
            }

            var oldKey = LicenseKey;
            if (string.IsNullOrEmpty(code))
            {
                var fragments = licenseKey.Split('-');
                var data = QueryData(string.Format("{0}?wc-api=serial-numbers-api&request=check&product_id={1}&serial_key={2}", BASE_URL, fragments[0], licenseKey));
                DateTime? expire = null;
                if (data?["expire_date"] != null)
                {
                    var strexpire = (string?)data?["expire_date"];
                    if (!string.IsNullOrEmpty(strexpire))
                    {
                        expire = DateTime.Parse(strexpire);
                    }
                }
                data = QueryData(string.Format("{0}?wc-api=serial-numbers-api&request=activate&product_id={1}&serial_key={2}&instance={3}&email={4}&platform={5}", BASE_URL, fragments[0], licenseKey, GetUUID(), HttpUtility.UrlEncode(email), Environment.OSVersion), new[] { "instance_already_activated" });

                var msg = (string?)(data?["message"]);
                log.Info(string.Format("Registration succeeded with message: {0}.", msg));

                LicenseKey = licenseKey;
                ExpirationDate = expire;
                Code = ComputeCode();

                SaveToFile();
            }
            else
            {
                LicenseKey = licenseKey;
                if (ParseCode(code))
                {
                    SaveToFile();
                }
                else
                {
                    LicenseKey = oldKey;
                    var error = "Invalid verification code.";
                    log.Error(error);
                    throw new MaintenanceException(error);
                }
            }
        }

        public string? ComputeCode()
        {
            return ComputeCode(null);
        }

        public string? ComputeCode(string? uuid)
        {
            if (!string.IsNullOrEmpty(LicenseKey))
            {
                var expiration = string.Empty;
                if (ExpirationDate != null)
                {
                    expiration = ExpirationDate.Value.ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                var key = GetUUIDKey(uuid);
                if (key != null)
                {
                    var aes = Aes.Create();
                    var iv = new byte[16];
                    aes.Key = key;
                    return Convert.ToHexString(aes.EncryptCbc(Encoding.UTF8.GetBytes(String.Format("LEO:{0}:{1}", LicenseKey, expiration)), iv));
                }
            }

            return null;
        }

        public bool ParseCode(string? code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var key = GetUUIDKey();
                if (key != null)
                {
                    var aes = Aes.Create();
                    var iv = new byte[16];
                    aes.Key = key;
                    var chain = Encoding.UTF8.GetString(aes.DecryptCbc(Convert.FromHexString(code), iv));
                    var fragments = chain.Split(':');
                    if (fragments.Length == 3 && fragments[0] == "LEO")
                    {
                        if (LicenseKey == fragments[1])
                        {
                            ExpirationDate = !string.IsNullOrEmpty(fragments[2]) ? DateTime.ParseExact(fragments[2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture) : null;
                            Code = code;
                            return true;
                        }
                        else
                        {
                            LicenseKey = null;
                        }
                    }
                }
            }

            return false;
        }

        private static byte[]? GetUUIDKey()
        {
            return GetUUIDKey(null);
        }

        private static byte[]? GetUUIDKey(string? uuid)
        {
            if (string.IsNullOrEmpty(uuid))
            {
                uuid = GetUUID();
            }

            if (!string.IsNullOrEmpty(uuid))
            {
#pragma warning disable SYSLIB0041 // Type or member is obsolete
                var deriv = new Rfc2898DeriveBytes(uuid, Encoding.UTF8.GetBytes("Security Freedom"));
#pragma warning restore SYSLIB0041 // Type or member is obsolete
                return deriv.GetBytes(16);
            }

            return null;
        }

        public static string? GetUUID()
        {
            string? uuid = null;
            var settings = AppSettings.GetSingletonInstance();
            if (settings.InstallationId.Length > 12)
            {
                // Be sure to save the InstallationId
                if (!File.Exists(settings.GetConfigFilePath()))
                {
                    settings.SaveToFile();
                }
                uuid = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", settings.InstallationId, Environment.MachineName))));
            }
            return uuid;
        }

        public static void OpenSubscription()
        {
            if (!string.IsNullOrEmpty(LeosacAppInfo.Instance?.ApplicationUrl))
            {
                var ps = new ProcessStartInfo(LeosacAppInfo.Instance.ApplicationUrl)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            }
        }

        public static void OpenRegistration()
        {
            var dialog = new PlanRegistrationWindow();
            dialog.ShowDialog();
        }

        public static string GetPlanUrl(string key)
        {
            return string.Format("{0}/show-plan?serial_key={1}", BASE_URL, key);
        }

        public static string GetOfflineRegistrationUrl(string? key, string? uuid)
        {
            var url = string.Format("{0}register-plan?", BASE_URL);
            if (!string.IsNullOrEmpty(key))
            {
                url += string.Format("serial_key={0}&", key);
            }
            if (!string.IsNullOrEmpty(uuid))
            {
                url += string.Format("instance={0}", uuid);
            }
            return url;
        }
    }
}
