using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                await appManager.RunAsync();
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
