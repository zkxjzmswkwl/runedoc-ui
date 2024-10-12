using CommunityToolkit.Mvvm.ComponentModel;

namespace RuneDocMVVM.ViewModels;

public partial class DebugPageViewModel : ViewModelBase
{
    [ObservableProperty] private double _red;
    [ObservableProperty] private double _green;
    [ObservableProperty] private double _blue;
}