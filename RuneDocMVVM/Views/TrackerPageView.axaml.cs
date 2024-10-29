using System;
using System.ComponentModel;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;

namespace RuneDocMVVM.Views;

public partial class TrackerPageView : UserControl
{
    public Thread? TrackerThread { get; private set; }
    public bool ShouldUpdateTrackers { get; private set; }
    
    public TrackerPageView()
    {
        InitializeComponent();
        ShouldUpdateTrackers = true;
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
         if (TrackerThread is not null)
         {
             ShouldUpdateTrackers = false;
             TrackerThread.Join();
         }
    }

    private void GetTrackers(object? sender, RoutedEventArgs e)
    {
        if (TrackerThread is not null)
        {
            Console.WriteLine("Tracker thread already running.");
            return;
        }
        
        var client = App.Provider.GetService<Client>()!;
        TrackerThread = new Thread(new ThreadStart(() =>
        {
            while (ShouldUpdateTrackers)
            {
                 client.SendData("req:metrics");
                 Thread.Sleep(1000);               
            }
        }));
        TrackerThread.Start();
    }
}