using System;
using System.Text;
using SuperSimpleTcp;

namespace RuneDocMVVM;

public class Client
{
    public SimpleTcpClient simpleTcpClient;
    
    public Client()
    {
        Console.WriteLine("Client ctor");
        simpleTcpClient = new SimpleTcpClient("127.0.0.1:6968");

        simpleTcpClient.Events.Connected += OnConnect;
        simpleTcpClient.Events.Disconnected += OnDisconnect;
        // simpleTcpClient.Events.DataReceived += DataReceived;
    }

    public void Connect()
    {
        simpleTcpClient.Connect();
    }

    public bool Connected
    {
        get => simpleTcpClient.IsConnected;
    }

    public void SendData(string data)
    {
        simpleTcpClient.Send($"{data}<dongs>");
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