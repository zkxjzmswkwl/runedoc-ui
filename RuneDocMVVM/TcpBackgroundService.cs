using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace RuneDocMVVM;

public class TcpClientService
{
    private static readonly Lazy<TcpClientService> _instance =
        new Lazy<TcpClientService>(() => new TcpClientService());

    public static TcpClientService Instance => _instance.Value;

    private TcpClient _client;
    private NetworkStream _stream;
    private CancellationTokenSource _cts;

    // Event to notify when data is received
    public event Action<string> DataReceived;

    // Private constructor to prevent instantiation
    private TcpClientService() { }

    public async Task StartClientAsync(string host, int port)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(host, port);
        _stream = _client.GetStream();

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        // Start reading data in a background task
        Task.Run(() => ReadDataAsync(token), token);
    }

    public Task StopClientAsync()
    {
        _cts?.Cancel();
        _stream?.Close();
        _client?.Close();
        return Task.CompletedTask;
    }

    public async Task SendDataAsync(string data)
    {
        if (_stream != null && _stream.CanWrite)
        {
            byte[] bytes = Encoding.UTF8.GetBytes($"{data}<dongs>");
            await _stream.WriteAsync(bytes);
            await _stream.FlushAsync();
        }
    }

    private async Task ReadDataAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (_stream.CanRead)
                {
                    
                    var buffer = new byte[4096];
                     var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                     if (bytesRead == 0) continue;
                     
                     var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                     
                     Dispatcher.UIThread.Post(() => DataReceived?.Invoke(receivedData));                   
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ReadDataAsync exception: {ex.Message}");
        }
    }
}