using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TCPDataValidator
{
    /// <summary>
    /// Основной класс управления приложением
    /// </summary>
    public class ApplicationManager : IDisposable
    {
        private readonly Config _config;
        private readonly ConcurrentDictionary<string, (string Data1, string Data2)> _dataDictionary;
        private readonly TcpResultServer _resultServer;
        private readonly List<TcpClientHandler> _clientHandlers = new List<TcpClientHandler>();
        private bool _disposed;

        public ApplicationManager(Config config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _dataDictionary = new ConcurrentDictionary<string, (string, string)>();
            _resultServer = new TcpResultServer(_config.ResultServerPort);
        }

        /// <summary>
        /// Основной цикл работы приложения
        /// </summary>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            // Запуск сервера результатов
            var resultServerTask = _resultServer.StartAsync(cancellationToken);

            // Параллельное подключение ко всем серверам
            var connectTasks = _config.Servers.Select(server =>
                ProcessServerAsync(server, cancellationToken)).ToList();

            await Task.WhenAll(connectTasks);

            // Обработка и сравнение данных
            ProcessAndSendResults();

            // Ожидание завершения работы сервера результатов
            await resultServerTask;
        }

        private async Task ProcessServerAsync(Config.ServerConfig server, CancellationToken cancellationToken)
        {
            var clientHandler = new TcpClientHandler(
                server.IpAddress,
                server.Port,
                server.TimeoutMs,
                server.RetryCount);

            _clientHandlers.Add(clientHandler);

            try
            {
                string data = await clientHandler.ReceiveDataAsync(cancellationToken);
                var processedData = PacketProcessor.ProcessPacket(data);

                _dataDictionary.AddOrUpdate(
                    $"{server.IpAddress}:{server.Port}",
                    processedData,
                    (key, oldValue) => processedData);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Данные получены от {server.IpAddress}:{server.Port}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка при обработке сервера {server.IpAddress}:{server.Port}: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void ProcessAndSendResults()
        {
            var dataList = _dataDictionary.Values.ToList();
            if (dataList.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Предупреждение: не получены данные ни от одного сервера");
                Console.ResetColor();
                return;
            }

            var result = DataComparer.CompareData(dataList);
            string resultPacket = $"#90#010102#27{result.Data1};{result.Data2}#91";

            // Асинхронная отправка без ожидания
            _ = _resultServer.BroadcastResultAsync(resultPacket)
                .ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Результат отправлен: {resultPacket}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Ошибка отправки результата: {t.Exception?.Message}");
                        Console.ResetColor();
                    }
                });
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _resultServer.Dispose();

            foreach (var handler in _clientHandlers)
            {
                handler.Dispose();
            }
        }
    }
}