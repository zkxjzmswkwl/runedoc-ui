using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using RuneDocMVVM.CommunicationTypes;

namespace RuneDocMVVM.ViewModels;

public partial class WardenPageViewModel : ViewModelBase
{
    private ObservableCollection<WatchedMessage> _wardenList = [];
    
    public WardenPageViewModel()
    {
        Console.WriteLine("WARDENPAGEVIEWMODEL CTOR");
    }

    public ObservableCollection<WatchedMessage> WardenList 
    {
        get => _wardenList;
        set
        {
            _wardenList = value;
            OnPropertyChanged(nameof(WardenList));
        }
    }
    
    public void AddMessagesThreadSafe(List<WatchedMessage> newList)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            WardenList = new ObservableCollection<WatchedMessage>(newList);
            // WardenList.Clear();
            // foreach (var wardenMessage in newList)
            // {
            //     Console.WriteLine(wardenMessage);
            //     WardenList.Add(wardenMessage);
            // }
        });
    }
}