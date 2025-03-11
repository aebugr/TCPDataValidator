using System;
using System.IO;
using Newtonsoft.Json;

namespace TCPDataValidator
{
    public class ConfigurationModule
    {
        public class ServerConfig
        {
            public string IpAddress { get; set; }
            public int Port { get; set; }
        }
        public class AppConfig
        {
            public ServerConfig[] Servers { get; set; }
            public int ResultServerPort { get; set; }
        }
        public AppConfig LoadConfig(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException("Файл конфигурации не найден.");
            }

            string json = File.ReadAllText(configFilePath);
            return JsonConvert.DeserializeObject<AppConfig>(json);
        }
        public void SaveConfig(AppConfig config, string configFilePath)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configFilePath, json);
        }
    }
}
