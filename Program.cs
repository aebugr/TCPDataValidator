using System;
using System.Threading;
using System.Threading.Tasks;

namespace TCPDataValidator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var config = Config.LoadConfig("config.json");
                var appManager = new ApplicationManager(config);

                // Создаём CancellationTokenSource для управления завершением работы
                using (var cts = new CancellationTokenSource())
                {
                    // Настраиваем обработчик для Ctrl+C
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        e.Cancel = true; // Предотвращаем немедленное завершение
                        cts.Cancel();    // Посылаем сигнал отмены
                        Console.WriteLine("\nЗавершение работы...");
                    };

                    await appManager.RunAsync(cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Работа программы завершена по запросу пользователя");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}