using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPDataValidator
{
    public class TcpResultServer 
    {
        private int _port; 
        public TcpResultServer(int port)
        {
            _port = port;
        }

        public async Task SendResultAsync(string result) // Метод для отправки данных
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, _port);
            listener.Start(); 

            using (TcpClient client = await listener.AcceptTcpClientAsync())
            {
                NetworkStream stream = client.GetStream(); // Получение потока данных
                byte[] buffer = Encoding.ASCII.GetBytes(result); // Преобразование строки в байты
                await stream.WriteAsync(buffer, 0, buffer.Length); // Отправление данных клиенту
            }
            listener.Stop(); 
        }
    }
}
