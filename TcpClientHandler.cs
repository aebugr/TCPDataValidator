using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class TcpClientHandler 
    {
        private string _ipAddress; 
        private int _port; 

        public TcpClientHandler(string ipAddress, int port) 
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public async Task<string> ReceiveDataAsync() // Метод для получения данных от сервера
        {
            try
            {
                using (TcpClient client = new TcpClient()) 
                {
                    await client.ConnectAsync(_ipAddress, _port).WaitAsync(TimeSpan.FromSeconds(10)); // Подключение к серверу с тайм-аутом 10 секунд

                    NetworkStream stream = client.GetStream(); // Получение потока данных
                    byte[] buffer = new byte[1024]; // Буфер для чтения данных
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // Чтение данных
                    return Encoding.ASCII.GetString(buffer, 0, bytesRead); 
                }
            }
            catch (SocketException ex) 
            {
                throw new ApplicationException($"Ошибка подключения к серверу {_ipAddress}:{_port}: {ex.Message}");
            }
            catch (TimeoutException ex)
            {
                throw new ApplicationException($"Тайм-аут подключения к серверу {_ipAddress}:{_port}: {ex.Message}");
            }
            catch (Exception ex) 
            {
                throw new ApplicationException($"Ошибка при получении данных: {ex.Message}");
            }
        }
    }
}