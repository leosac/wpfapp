using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Leosac.WpfApp
{
    public abstract class PermanentConfig<T> where T : PermanentConfig<T>, new()
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        static JsonSerializerSettings _jsonSettings;

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

        public abstract string GetDefaultFileName();

        public virtual void SaveToFile()
        {
            SaveToFile(GetConfigFilePath(true));
        }

        public void SaveToFile(string filePath)
        {
            log.Info("Saving configuration to file...");
            var serialized = JsonConvert.SerializeObject(this, _jsonSettings);
            System.IO.File.WriteAllText(filePath, serialized, Encoding.UTF8);
            log.Info("Configuration saved.");
        }

        public static T? LoadFromFile(string filePath)
        {
            log.Info("Loading configuration from file...");
            if (System.IO.File.Exists(filePath))
            {
                var serialized = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
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

        public string GetConfigFilePath(bool createFolders = false)
        {
            return GetConfigFilePath(GetDefaultFileName(), createFolders);
        }

        public static string GetConfigFilePath(string fileName, bool createFolders = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appData, "Leosac");
            if (!Directory.Exists(path) && createFolders)
                Directory.CreateDirectory(path);

            path = Path.Combine(path, LeosacAppInfo.Instance.ApplicationName);
            if (!Directory.Exists(path) && createFolders)
                Directory.CreateDirectory(path);

            return Path.Combine(path, fileName);
        }
    }
}
