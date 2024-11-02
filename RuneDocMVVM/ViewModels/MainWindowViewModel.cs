using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SuperSimpleTcp;
using System.Text.RegularExpressions;
using Avalonia.Threading;
using RuneDocMVVM.CommunicationTypes;
using RuneDocMVVM.Models;
using RuneDocMVVM.Views;

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
    
    private readonly Dictionary<Type, ViewModelBase> _pageInstances;

    private bool _recvOpeningMessage = false;

    public MainWindowViewModel()
    {
        _pageInstances = new Dictionary<Type, ViewModelBase>
        {
            { typeof(HomePageViewModel), new HomePageViewModel() },
            { typeof(DebugPageViewModel), App.Provider.GetService<DebugPageViewModel>()! },
            { typeof(TrackerPageViewModel), App.Provider.GetService<TrackerPageViewModel>()! },
            { typeof(WardenPageViewModel), App.Provider.GetService<WardenPageViewModel>()! },
            { typeof(PluginHostViewModel), App.Provider.GetService<PluginHostViewModel>()! }
        };
        App.Provider.GetService<Client>()!.simpleTcpClient.Events.DataReceived += DataReceived;
        
        Dispatcher.UIThread.Post(() =>
        {
            string str = @"
                {
                  ""PluginName"": ""SamplePlugin"",
                  ""Version"": ""1.0"",
                  ""Ui"": {
                    ""layout"": ""StackPanel"",
                    ""orientation"": ""Vertical"",
                    ""children"": [
                      {
                        ""type"": ""TextBlock"",
                        ""properties"": {
                          ""Text"": ""Welcome to Sample Plugin"",
                          ""FontSize"": 20,
                          ""Foreground"": ""#FF0000""
                        }
                      },
                      {
                        ""type"": ""Button"",
                        ""properties"": {
                          ""Content"": ""Click Me"",
                          ""Command"": ""SampleCommand"",
                          ""Background"": ""Green""
                        }
                      },
                      {
                        ""type"": ""ProgressBar"",
                        ""properties"": {
                          ""Minimum"": 0,
                          ""Maximum"": 100,
                          ""Value"": 50
                        }
                      }
                    ]
                  }
                }
            ";
            var plugin = PluginModel.FromJson(str);
            App.Provider.GetService<PluginHostViewModel>()!.AddPlugin(plugin);
        });
    }

    // Giga shit. Don't care.
    private void DataReceived(object? sender, DataReceivedEventArgs e)
    {
        var packet = Encoding.UTF8.GetString(e.Data.Array, 0, e.Data.Count);
        Console.WriteLine(packet);
        
        if (!_recvOpeningMessage)
        {
            _recvOpeningMessage = true;
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
        
        if (packet.StartsWith("_specpl_"))
        {
            var spl = packet.Split(":");
            if (spl[1] == "afkwarden")
            {
                 if (spl[2] == "playalert")
                 {
                     var wardenList = App.Provider.GetService<WardenPageViewModel>()!.WardenList;
                     WatchedMessage alertMessage = null;
                     foreach (var msg in wardenList)
                     {
                         if (msg.Message.Equals(spl[3]))
                         {
                             alertMessage = msg;
                         }
                     }

                     if (alertMessage is { TextToSpeech: true })
                     {
                         Speech.Say(alertMessage.TextToSpeechMessage);
                     }
                     else
                     {
                          var audioPlayer = App.Provider.GetService<AudioPlayer>()!;
                          audioPlayer.Play("fbalert.mp3");                        
                     }

                 }
                 else if (spl[2] == "queryresp")
                 {
                     var messages = Regex.Split(spl[3], "\\^")[0..^1];
                     List<WatchedMessage> messageList = [];
                     
                     foreach (var message in messages)
                     {
                         messageList.Add(WatchedMessage.Create(message));
                     }
                     
                     var vm = App.Provider.GetService<WardenPageViewModel>()!;
                     vm.AddMessagesThreadSafe(messageList);
                 }
            }
        }
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;

        if (_pageInstances.TryGetValue(value.ModelType, out var instance))
        {
            CurrentPage = instance;
        }
        else
        {
            var newInstance = (ViewModelBase)Activator.CreateInstance(value.ModelType)!;
            _pageInstances[value.ModelType] = newInstance;
            CurrentPage = newInstance;
        }
    }
    public ObservableCollection<ListItemTemplate> Items { get; } = new()
    {
        new ListItemTemplate(typeof(HomePageViewModel), "Home"),
        new ListItemTemplate(typeof(DebugPageViewModel), "Bug"),
        new ListItemTemplate(typeof(TrackerPageViewModel), "DataBar"),
        new ListItemTemplate(typeof(WardenPageViewModel), "Keyboard"),
        new ListItemTemplate(typeof(PluginHostViewModel), "Keyboard")
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