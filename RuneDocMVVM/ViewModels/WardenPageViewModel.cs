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
    
    // Modal
    private object _selectedItem;
    public ObservableCollection<string> Items { get; set; } = ["Ping", "Text to speech"];
    public WatchedMessage DialogSubject { get; set; }

    public WardenPageViewModel()
    {
        Console.WriteLine("WARDENPAGEVIEWMODEL CTOR");
        _selectedItem = Items[0];
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


    public object SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem != value)
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                OnSelectionChanged(_selectedItem);
            }
        }
    }
    
    private void OnSelectionChanged(object selectedItem)
    {
    }
    
    public void AddMessagesThreadSafe(List<WatchedMessage> newList)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            WardenList = new ObservableCollection<WatchedMessage>(newList);
        });
    }
}