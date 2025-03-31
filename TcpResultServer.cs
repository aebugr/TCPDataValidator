using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPDataValidator
{
    /// <summary>
    /// TCP-сервер для отправки результатов проверки
    /// </summary>
    public class TcpResultServer : IDisposable
    {
        private readonly int _port;
        private TcpListener _listener;
        private readonly List<TcpClient> _connectedClients = new List<TcpClient>();
        private readonly object _lock = new object();
        private bool _isRunning;

        public TcpResultServer(int port)
        {
            _port = port;
        }

        /// <summary>
        /// Запуск сервера результатов
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _isRunning = true;
            Console.WriteLine($"Сервер результатов запущен на порту {_port}");

            try
            {
                while (_isRunning && !cancellationToken.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync()
                        .WithCancellation(cancellationToken);

                    lock (_lock)
                    {
                        _connectedClients.Add(client);
                    }
                    Console.WriteLine($"Новое подключение: {client.Client.RemoteEndPoint}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Сервер результатов остановлен");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка сервера результатов: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Рассылка результатов всем подключенным клиентам
        /// </summary>
        public async Task BroadcastResultAsync(string result)
        {
            // Копируем список клиентов под lock
            List<TcpClient> clientsToProcess;
            lock (_lock)
            {
                clientsToProcess = new List<TcpClient>(_connectedClients);
            }

            List<TcpClient> clientsToRemove = new List<TcpClient>();
            byte[] buffer = Encoding.ASCII.GetBytes(result);

            foreach (var client in clientsToProcess)
            {
                try
                {
                    if (client.Connected)
                    {
                        var stream = client.GetStream();
                        await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    }
                    else
                    {
                        clientsToRemove.Add(client);
                    }
                }
                catch
                {
                    clientsToRemove.Add(client);
                }
            }

            // Удаление отключенных клиентов под lock
            if (clientsToRemove.Count > 0)
            {
                lock (_lock)
                {
                    foreach (var client in clientsToRemove)
                    {
                        client.Dispose();
                        _connectedClients.Remove(client);
                    }
                }
            }
        }

        public void Dispose()
        {
            _isRunning = false;
            lock (_lock)
            {
                foreach (var client in _connectedClients)
                {
                    client.Dispose();
                }
                _connectedClients.Clear();
            }
            _listener?.Stop();
        }
    }

    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            }
            return await task;
        }
    }
}