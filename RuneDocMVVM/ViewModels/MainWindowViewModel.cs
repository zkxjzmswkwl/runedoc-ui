using System;
using System.Collections.Generic;
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
using System.Text.RegularExpressions;
using RuneDocMVVM.CommunicationTypes;

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
        var packet = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
        Console.WriteLine(packet);
        
        if (!_recvOpeningMessage)
        {
            _recvOpeningMessage = true;
            App.Provider.GetService<Client>()!.SendData("req:rsn");
        }

        if (packet.StartsWith("resp:"))
        {
            var spl = packet.Split(":");
            // Obviously we crash if this doesn't split into at least 0..2.
            // But frankly if we can't get this right, we deserve to crash.
            switch (spl[1])
            {
                case "rsn":
                    WindowTitle = $"RuneDoc - {spl[2]}";
                    break;
                // This is all bad.
                case "sceneobjects":
                    // Splits the string by ^ (delimiter used to separate entities in buffer).
                    // Rather than keeping ^ in the splits, it's removed entirely with only other characters
                    // remaining.
                    var entities = Regex.Split(spl[2], "\\^")[0..^1];
                    var debugVm = App.Provider.GetService<DebugPageViewModel>()!;
                    List<Entity> entityList = [];
                    
                    foreach (var entity in entities)
                    {
                        var ent = Entity.FromString(entity);
                        entityList.Add(ent);
                    }
                    
                    debugVm.AddEntitiesThreadSafe(entityList);
                    break;
                case "nodes":
                    // Splits the string by ^ (delimiter used to separate entities in buffer).
                    // Rather than keeping ^ in the splits, it's removed entirely with only other characters
                    // remaining.
                    var nodes = Regex.Split(spl[2], "\\^")[0..^1];
                    
                    List<Node> nodeList = [];
                    
                    foreach (var node in nodes)
                    {
                        var obj = Node.FromString(node);
                        nodeList.Add(obj);
                    }
                    
                    App.Provider.GetService<DebugPageViewModel>()!.AddNodesThreadSafe(nodeList);
                    break;
                case "metrics":
                    var trackers = Regex.Split(spl[2], "\\^")[0..^1];
                    var trackerVm = App.Provider.GetService<TrackerPageViewModel>()!;
                    List<Tracker> trackerList = [];

                    foreach (var tracker in trackers)
                    {
                        var a = Tracker.FromString(tracker);
                        trackerList.Add(a);
                    }
                    
                    trackerVm.AddTrackersThreadSafe(trackerList);
                    break;
            }
        }
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;
        
        var debugVm = App.Provider.GetService<DebugPageViewModel>()!;
        // This is giga retarded
        if (value.ModelType != typeof(DebugPageViewModel) && value.ModelType != typeof(TrackerPageViewModel))
        {
             var instance = Activator.CreateInstance(value.ModelType);
             if (instance is null) return;           
            CurrentPage = (ViewModelBase)instance;
            return;
        }

        if (value.ModelType == typeof(DebugPageViewModel))
        {
            CurrentPage = (ViewModelBase)debugVm;
            return;
        }
        
        // incredibly retarded.
        CurrentPage = (ViewModelBase)App.Provider.GetService<TrackerPageViewModel>()!;
    }
    
    public ObservableCollection<ListItemTemplate> Items { get; } = new()
    {
        new ListItemTemplate(typeof(HomePageViewModel), "Home"),
        new ListItemTemplate(typeof(DebugPageViewModel), "Bug"),
        new ListItemTemplate(typeof(TrackerPageViewModel), "DataBar")
    };

    [RelayCommand]
    private void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
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