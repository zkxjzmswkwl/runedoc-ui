using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using DialogHostAvalonia;
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
    
    private async void OnListBoxPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not TextBlock textBlock) return;
        var listBoxItem = textBlock.FindAncestorOfType<ListBoxItem>();
        if (listBoxItem == null) return;
        var index = MessageListing.ItemContainerGenerator.IndexFromContainer(listBoxItem);
        if (index < 0 || index >= MessageListing.ItemCount) return;
        var item = MessageListing.Items.ElementAt(index);

        var point = e.GetCurrentPoint(textBlock);

        if (point.Properties.IsMiddleButtonPressed)
        {
            HandleMiddleClickOnItem(item);
        }
        else if (point.Properties.IsRightButtonPressed)
        {
            HandleRightClickOnItem(item);
        }
    }

    private void  HandleRightClickOnItem(object? item)
    {
        if (item != null)
        {
            var message = (WatchedMessage)item;
            App.Provider.GetService<WardenPageViewModel>()!.DialogSubject = message;
            WardenDialogHost.IsOpen = true;
            Console.WriteLine(message.Message);
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

    private void Warden_OnDialogClosing(object? sender, DialogClosingEventArgs e)
    {
        Console.WriteLine(sender);
        var selectedItem = (ComboBoxItem)e.Parameter!;
        if (selectedItem.Content == null) return;
        var vm = App.Provider.GetService<WardenPageViewModel>()!;

        bool isTts = selectedItem.Content.Equals("Text to speech");
        string ttsString = "boner";
        
        foreach (var msg in vm.WardenList)
        {
            if (msg.Message.Equals(vm.DialogSubject.Message))
            {
                msg.TextToSpeech = isTts;
                msg.TextToSpeechMessage = ttsString;
            }
        }

        Console.WriteLine($"Closing dialog with parameter: {selectedItem.Content}");
    }
    
    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            var selectedItem = comboBox.SelectedItem;
            Console.WriteLine(selectedItem);
        }
    }

    private void OnTap(object? sender, TappedEventArgs e)
    {
        Console.WriteLine(e);
    }
}
