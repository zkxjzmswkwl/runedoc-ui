using CommunityToolkit.Mvvm.ComponentModel;

using RuneDocMVVM.Models;
using System.Collections.ObjectModel;

namespace RuneDocMVVM.ViewModels
{
    public partial class PluginHostViewModel : ViewModelBase
    {
        public ObservableCollection<PluginModel> Plugins { get; } = new();

        public void AddPlugin(PluginModel plugin)
        {
            Plugins.Add(plugin);
        }
    }
}