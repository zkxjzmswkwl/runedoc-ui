using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace RuneDocMVVM.Views;

public partial class HomePageView : UserControl
{
    public HomePageView()
    {
        InitializeComponent();
    }

    private void Connect(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>();
        if (!client.Connected)
        {
            client.Connect();
        }
    }
}