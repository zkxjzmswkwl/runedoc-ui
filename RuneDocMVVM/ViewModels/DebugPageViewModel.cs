using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using RuneDocMVVM.CommunicationTypes;

namespace RuneDocMVVM.ViewModels;

public partial class DebugPageViewModel : ViewModelBase
{
    private ObservableCollection<Entity> _entities = [];
    private ObservableCollection<Node> _nodes = [];

    public DebugPageViewModel()
    {
        Console.WriteLine("DEBUGPAGEVIEWMODEL CTOR");
    }
    
    [ObservableProperty] private double _red;
    [ObservableProperty] private double _green;
    [ObservableProperty] private double _blue;
    
    public ObservableCollection<Entity> Entities
    {
        get => _entities;
        set
        {
            _entities = value;
            OnPropertyChanged(nameof(Entities));
        }
    }
    
    public ObservableCollection<Node> Nodes 
    {
        get => _nodes;
        set
        {
            _nodes = value;
            OnPropertyChanged();
        }
    }
    
    public void AddEntitiesThreadSafe(List<Entity> entities)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Entities.Clear();
            foreach (var entity in entities)
            {
                Entities.Add(entity);
            }
        });
    }
    
    
    public void AddNodesThreadSafe(List<Node> nodes)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Nodes.Clear();
            foreach (var node in nodes)
            {
                Nodes.Add(node);
            }
        });
    }
}