using Microsoft.Extensions.DependencyInjection;
using RuneDocMVVM.ViewModels;

namespace RuneDocMVVM;

public static class ServiceCollectionExtensions {
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddSingleton<DebugPageViewModel>();
        collection.AddSingleton<TrackerPageViewModel>();
        collection.AddSingleton<Client>();
    }
}