using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test;

namespace TCPDataValidator
{
    public class ApplicationManager
    {
        private Config _config;

        public ApplicationManager(Config config)
        {
            _config = config;
        }

        public async Task RunAsync() // Асинхронный метод RunAsync, который управляет всей логикой приложения
        {
            List<(string Data1, string Data2)> dataList = new List<(string Data1, string Data2)>();

            foreach (var server in _config.Servers)
            {
                var client = new TcpClientHandler(server.IpAddress, server.Port);
                try
                {
                    string data = await client.ReceiveDataAsync();
                    var processedData = PacketProcessor.ProcessPacket(data);
                    dataList.Add(processedData);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Успешно подключено к серверу {server.IpAddress}:{server.Port} и получены данные.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка подключения к серверу {server.IpAddress}:{server.Port}: {ex.Message}");
                }
                finally
                {
                    Console.ResetColor();
                }
            }
            var result = DataComparer.CompareData(dataList);
            string resultPacket = $"#90#010102#27{result.Data1};{result.Data2}#91";
            var resultServer = new TcpResultServer(_config.ResultServerPort);
            try
            {
                await resultServer.SendResultAsync(resultPacket);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Результат отправлен: {resultPacket}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка при отправке результата: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
            }

            // Задержка перед завершением работы (3 минуты)
            Console.WriteLine("Ожидание завершения работы...");
            await Task.Delay(180000);
        }
    }
}
