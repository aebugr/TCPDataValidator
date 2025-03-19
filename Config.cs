using System;
using System.IO;
using Newtonsoft.Json;

namespace TCPDataValidator
{
    public class Config
    {
        public class ServerConfig // Вложенный класс для описания одного сервера
        {
            public string IpAddress { get; set; }
            public int Port { get; set; }
        }
        public ServerConfig[] Servers { get; set; }
        public int ResultServerPort { get; set; }
        public static Config LoadConfig(string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Config>(json); // Десериализация JSON в объект Config
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка загрузки конфигурации: {ex.Message}", ex);
            }
        }
    }
}