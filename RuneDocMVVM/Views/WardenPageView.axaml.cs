using System;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;

using RuneDocMVVM;
using RuneDocMVVM.CommunicationTypes;
using RuneDocMVVM.ViewModels;

namespace RuneDocMVVM.Views;

public partial class WardenPageView : UserControl
{
    public Thread? WardenThread { get; private set; }
    public bool ShouldRunWardenThread { get; private set; }
    
    public WardenPageView()
    {
        InitializeComponent();
        ShouldRunWardenThread = true;
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
         if (WardenThread is not null)
         {
             ShouldRunWardenThread = false;
             WardenThread.Join();
         }
    }

    private void StartAFKWarden(object? sender, RoutedEventArgs e)
    {
        if (WardenThread is not null)
        {
            Console.WriteLine("Warden thread already running.");
            return;
        }
        
        var client = App.Provider.GetService<Client>()!;
        WardenThread = new Thread(new ThreadStart(() =>
        {
            while (ShouldRunWardenThread)
            {
                 client.SendData("_specpl_:afkwarden:checkin");
                 Thread.Sleep(200); 
            }
        }));
        WardenThread.Start();
    }

    
    private void OnListBoxPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Check if it's a middle mouse button click
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed)
        {
            // Find the clicked ListBoxItem by traversing the visual tree
            var listBoxItem = (e.Source as Control)?.FindAncestorOfType<ListBoxItem>();
            if (listBoxItem != null)
            {
                // Get the item associated with this ListBoxItem by finding its index
                int index = MessageListing.ItemContainerGenerator.IndexFromContainer(listBoxItem);
                if (index >= 0 && index < MessageListing.ItemCount)
                {
                    var item = MessageListing.Items.ElementAt(index);

                    // Handle the middle-click on this item
                    HandleMiddleClickOnItem(item);
                }
            }
        }
    }
    
    private void HandleMiddleClickOnItem(object? item)
    {
        if (item != null)
        {
            var message = (WatchedMessage)item;
            App.Provider.GetService<Client>()!.SendData($"_specpl_:afkwarden:removeWatchedMessage:{message.Message}");
        }
    }

    private void AddMessage(object? sender, RoutedEventArgs e)
    {
        var client = App.Provider.GetService<Client>()!;
        client.SendData($"_specpl_:afkwarden:addWatchedMessage:{AddNewMessage.Text}");
        AddNewMessage.Text = "";
    }
}
