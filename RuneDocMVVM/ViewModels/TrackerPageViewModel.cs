using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using RuneDocMVVM.CommunicationTypes;

namespace RuneDocMVVM.ViewModels;

public partial class TrackerPageViewModel : ViewModelBase
{
    private ObservableCollection<Tracker> _trackers = [];
    public ObservableCollection<Tracker> Trackers
    {
        get => _trackers;
        set
        {
            _trackers = value;
            OnPropertyChanged(nameof(Trackers));
        }
    }
    
    public TrackerPageViewModel()
    {
        Console.WriteLine("TRACKERPAGEVIEWMODEL CTOR");
    }
    
    public void AddTrackersThreadSafe(List<Tracker> trackers)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Trackers.Clear();
            foreach (var tracker in trackers)
            {
                Trackers.Add(tracker);
            }
        });
    }
}
