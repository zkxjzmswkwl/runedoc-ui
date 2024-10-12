using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Avalonia.Rendering;
using FluentAvalonia.UI.Windowing;
using RuneDocMVVM.ViewModels;
using SuperSimpleTcp;

namespace RuneDocMVVM.Views;

public partial class MainWindow : AppWindow
{
    
    public MainWindow()
    {
        InitializeComponent();
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private async void MainWindow_Closed(object? sender, EventArgs e)
    {
        
    }

    private async void MainWindow_Loaded(object sender, EventArgs e)
    {
        // try
        // {
        //     await TcpClientService.Instance.StartClientAsync("localhost", 6968);
        //     await TcpClientService.Instance.SendDataAsync("req:getRsn");
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine(ex.Message);
        // }
    }
    
    private void DataReceived(object? sender, DataReceivedEventArgs e)
    {
        Console.WriteLine($"[{e.IpPort}] {Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count)}");
    }

    private async void OnDataReceived(string buffer)
    {
        Console.WriteLine(buffer);
        if (buffer.StartsWith("rsn:"))
        {
            var rsn = buffer.Split("rsn:")[1];
            var vm = (MainWindowViewModel)DataContext;
            vm.WindowTitle = $"RuneDoc - {rsn}";
        }
    }
}