using System;
using System.IO;
using Newtonsoft.Json;

namespace TCPDataValidator
{
    /// <summary>
    /// Класс для работы с конфигурацией приложения
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Конфигурация сервера-источника данных
        /// </summary>
        public class ServerConfig
        {
            public string IpAddress { get; set; }
            public int Port { get; set; }
            public int TimeoutMs { get; set; } = 5000; // Добавлен таймаут
            public int RetryCount { get; set; } = 3; // Количество попыток подключения
        }

        /// <summary>
        /// Список серверов-источников данных
        /// </summary>
        public ServerConfig[] Servers { get; set; }

        /// <summary>
        /// Порт для сервера результатов
        /// </summary>
        public int ResultServerPort { get; set; }

        /// <summary>
        /// Загрузка конфигурации из JSON-файла
        /// </summary>
        public static Config LoadConfig(string path)
        {
            try
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException($"Конфигурационный файл {path} не найден");

                string json = File.ReadAllText(path);
                var config = JsonConvert.DeserializeObject<Config>(json);

                // Валидация конфигурации
                if (config.Servers == null || config.Servers.Length == 0)
                    throw new ApplicationException("Не указаны серверы в конфигурации");

                return config;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка загрузки конфигурации: {ex.Message}", ex);
            }
        }
    }
}