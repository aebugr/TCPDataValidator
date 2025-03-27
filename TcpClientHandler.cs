using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPDataValidator
{
    /// <summary>
    /// Обработчик TCP-клиента для подключения к серверам-источникам
    /// </summary>
    public class TcpClientHandler : IDisposable
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly int _timeoutMs;
        private readonly int _retryCount;
        private TcpClient _client;

        public TcpClientHandler(string ipAddress, int port, int timeoutMs, int retryCount)
        {
            _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            _port = port;
            _timeoutMs = timeoutMs;
            _retryCount = retryCount;
        }

        /// <summary>
        /// Получение данных с сервера с повторными попытками
        /// </summary>
        public async Task<string> ReceiveDataAsync(CancellationToken cancellationToken)
        {
            int attempt = 0;
            Exception lastException = null;

            while (attempt < _retryCount)
            {
                attempt++;
                try
                {
                    _client = new TcpClient();
                    var connectTask = _client.ConnectAsync(_ipAddress, _port);

                    if (await Task.WhenAny(connectTask, Task.Delay(_timeoutMs, cancellationToken)) == connectTask)
                    {
                        if (_client.Connected)
                        {
                            using (var stream = _client.GetStream())
                            {
                                var buffer = new byte[1024];
                                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                                return Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            }
                        }
                    }
                    else
                    {
                        throw new TimeoutException($"Таймаут подключения к {_ipAddress}:{_port}");
                    }
                }
                catch (Exception ex) when (attempt < _retryCount)
                {
                    lastException = ex;
                    await Task.Delay(1000, cancellationToken); // Задержка перед повторной попыткой
                }
                finally
                {
                    _client?.Dispose();
                }
            }

            throw new ApplicationException($"Не удалось подключиться к {_ipAddress}:{_port} после {_retryCount} попыток", lastException);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}