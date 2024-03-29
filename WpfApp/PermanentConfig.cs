﻿using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Leosac.WpfApp
{
    public abstract class PermanentConfig<T> : ObservableObject where T : PermanentConfig<T>, new()
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        static readonly JsonSerializerSettings _jsonSettings;

        static PermanentConfig()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                Formatting = Formatting.Indented
            };
        }

        public static string? ConfigDirectory { get; set; }

        public static string GetDefaultFileName()
        {
            return string.Format("{0}.json", typeof(T).Name);
        }

        public virtual void SaveToFile()
        {
            SaveToFile(GetConfigFilePath(true));
        }

        public void SaveToFile(string filePath)
        {
            log.Info("Saving configuration to file...");
            var serialized = JsonConvert.SerializeObject(this, _jsonSettings);
            File.WriteAllText(filePath, serialized, Encoding.UTF8);
            log.Info("Configuration saved.");
        }

        public static T? LoadFromFile()
        {
            return LoadFromFile(GetConfigFilePath(GetDefaultFileName()));
        }

        public static T? LoadFromFile(string filePath)
        {
            log.Info("Loading configuration from file...");
            if (File.Exists(filePath))
            {
                var serialized = File.ReadAllText(filePath, Encoding.UTF8);
                var config = JsonConvert.DeserializeObject<T>(serialized, _jsonSettings);
                log.Info("Configuration loaded from file.");
                return config;
            }
            else
            {
                log.Info("No file found, falling back to default instance.");
                return new T();
            }
        }

        public string GetConfigFilePath()
        {
            return GetConfigFilePath(false);
        }

        public string GetConfigFilePath(bool createFolders)
        {
            return GetConfigFilePath(GetDefaultFileName(), createFolders);
        }

        public static string GetConfigFilePath(string fileName)
        {
            return GetConfigFilePath(fileName, false);
        }

        public static string GetConfigFilePath(string fileName, bool createFolders)
        {
            string path;
            if (!string.IsNullOrEmpty(ConfigDirectory))
            {
                path = ConfigDirectory;
            }
            else
            {
                var appData = (LeosacAppInfo.Instance?.PerUserInstallation).GetValueOrDefault(true) ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                path = Path.Combine(appData, "Leosac");
                if (!Directory.Exists(path) && createFolders)
                {
                    Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, LeosacAppInfo.Instance?.ApplicationName ?? "Test");
            }
            if (!Directory.Exists(path) && createFolders)
            {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, fileName);
        }
    }
}
