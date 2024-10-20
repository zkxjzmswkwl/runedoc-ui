using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RuneDocMVVM.ViewModels;

namespace RuneDocMVVM.Views;

public partial class DebugPageView : UserControl
{
    public DebugPageView()
    {
        InitializeComponent();
    }

    private void ApplySilhouetteColours(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>();
        var vm = (DebugPageViewModel)DataContext;
        
        client.SendData($"cmd:red:{(long)vm.Red}");
        client.SendData($"cmd:green:{(long)vm.Green}");
        client.SendData($"cmd:blue:{(long)vm.Blue}");
    }

    private void RequestPrayer(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>();
        client.SendData("req:prayer");
    }

    private void RequestHealth(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>();
        client.SendData("req:health");
    }

    private void QueryScene(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>();
        client.SendData($"req:sceneobjects:{EntityNameSubstr.Text}");
    }
    
    private void QuerySceneNodes(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>();
        client.SendData($"req:nodes:{NodeNameSubstr.Text}");
    }
    private void HideNpcs(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>();
        client.SendData("req:hideentities:npc");
        HideNpcsButton.Content = (string)HideNpcsButton.Content! == "Unhide Npcs" ? "Hide Npcs" : "Unhide Npcs";
    }
    private void HidePlayers(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>();
        client.SendData("req:hideentities:entity");
        HidePlayersButton.Content = (string)HidePlayersButton.Content == "Unhide Players" ? "Hide Players" : "Unhide Players";
    }

    [DllImport("user32.dll")]
    internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll")]
    internal static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    internal static extern bool SetClipboardData(uint uFormat, IntPtr data);



}