using System;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;

namespace RuneDocMVVM;

public class Client
{
    public SimpleTcpClient simpleTcpClient;
    
    public Client()
    {
        Console.WriteLine("Client ctor");
        simpleTcpClient = new SimpleTcpClient("127.0.0.1:6969");
        var settings = new SimpleTcpClientSettings();
        settings.NoDelay = true;
        simpleTcpClient.Settings = settings;

        simpleTcpClient.Events.Connected += OnConnect;
        simpleTcpClient.Events.Disconnected += OnDisconnect;
    }

    public void Connect()
    {
        simpleTcpClient.ConnectWithRetries(2000);
        simpleTcpClient.Send("req:rsn<dongs>");
    }

    public bool Connected
    {
        get => simpleTcpClient.IsConnected;
    }

    public void SendData(string data)
    {
        simpleTcpClient.SendAsync($"{data}<dongs>");
    }
    
    public void Disconnect()
    {
        simpleTcpClient.Disconnect();
    }

    static void OnConnect(object sender, ConnectionEventArgs e)
    {
        Console.WriteLine($"*** Server {e.IpPort} connected");
    }

    static void OnDisconnect(object sender, ConnectionEventArgs e)
    {
        Console.WriteLine($"*** Server {e.IpPort} disconnected"); 
    }

    static void DataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.WriteLine($"[{e.IpPort}] {Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count)}");
    }
}