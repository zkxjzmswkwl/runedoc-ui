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
        DataContext = new DebugPageViewModel();
    }

    private void ApplySilhouetteColours(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>();
        var vm = (DebugPageViewModel)DataContext;
        
        client.SendData($"cmd:red:{(long)vm.Red}");
        client.SendData($"cmd:green:{(long)vm.Green}");
        client.SendData($"cmd:blue:{(long)vm.Blue}");
    }
}