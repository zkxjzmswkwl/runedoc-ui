using System;
using System.Net.Sockets;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RuneDocMVVM.ViewModels;
using RuneDocMVVM.Views;

namespace RuneDocMVVM;

public partial class App : Application
{
    // This shit seems so retarded.
    public static ServiceCollection Collection;
    public static ServiceProvider Provider;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        // {
        //     // Line below is needed to remove Avalonia data validation.
        //     // Without this line you will get duplicate validations from both Avalonia and CT
        //     BindingPlugins.DataValidators.RemoveAt(0);
        //     desktop.MainWindow = new MainWindow
        //     {
        //         DataContext = new MainWindowViewModel(),
        //     };
        // }
        //
        // base.OnFrameworkInitializationCompleted();
        // If you use CommunityToolkit, line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        // Register all the services needed for the application to run
        App.Collection = new ServiceCollection();
        App.Collection.AddCommonServices();

        // Creates a ServiceProvider containing services from the provided IServiceCollection
        App.Provider = App.Collection.BuildServiceProvider();

        var vm = App.Provider.GetRequiredService<MainWindowViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainWindow
            {
                DataContext = vm
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}