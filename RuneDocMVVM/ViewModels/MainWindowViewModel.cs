using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SuperSimpleTcp;

namespace RuneDocMVVM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPaneOpen = true;
    
    [ObservableProperty]
    public string _windowTitle = "RuneDoc";
    
    [ObservableProperty]
    private ViewModelBase _currentPage = new HomePageViewModel();
    
    [ObservableProperty]
    private string _textBlockName = "Dongs";

    [ObservableProperty]
    private ListItemTemplate _selectedListItem;

    private bool _recvOpeningMessage = false;

    public MainWindowViewModel()
    {
        App.Provider.GetService<Client>()!.simpleTcpClient.Events.DataReceived += DataReceived;
    }

    private void DataReceived(object? sender, DataReceivedEventArgs e)
    {
        Console.WriteLine(e.Data);
        if (!_recvOpeningMessage)
        {
            _recvOpeningMessage = true;
            App.Provider.GetService<Client>()!.SendData("req:getRsn");
        }

        var packet = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
        if (packet.StartsWith("rsn:"))
        {
            var rsn = packet.Replace("rsn:", "");
            WindowTitle = $"RuneDoc - {rsn}";
        }
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;
        var instance = Activator.CreateInstance(value.ModelType);
        if (instance is null) return;
        CurrentPage = (ViewModelBase)instance;
    }
    
    public ObservableCollection<ListItemTemplate> Items { get; } = new()
    {
        new ListItemTemplate(typeof(HomePageViewModel), "Home"),
        new ListItemTemplate(typeof(DebugPageViewModel), "Bug")
    };

    [RelayCommand]
    private void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
    
    [RelayCommand]
    private void ButtonOnClick()
    {
        TextBlockName = "Clicked";
    }
}

public class ListItemTemplate
{
    public ListItemTemplate(Type type, string iconKey)
    {
        ModelType = type;
        Label = type.Name.Replace("PageViewModel", "");
        Application.Current!.TryFindResource(iconKey, out var res);
        Icon = (StreamGeometry)res!;
    }
    
    public string Label { get;  }
    public Type ModelType { get; }
    public StreamGeometry Icon { get; }
}