using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering;
using FluentAvalonia.UI.Windowing;
using RuneDocMVVM.ViewModels;
using SuperSimpleTcp;

namespace RuneDocMVVM.Views;

public partial class MainWindow : AppWindow
{
    
    public MainWindow()
    {
        InitializeComponent();
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }
    
    private async Task FadeInBackground(ImageBrush imageBrush, int durationMs)
    {
        const double targetOpacity = 0.5;
        const double steps = 50;
        double interval = durationMs / steps;
        double opacityStep = targetOpacity / steps;

        for (int i = 0; i <= steps; i++)
        {
            imageBrush.Opacity = i * opacityStep;
            await Task.Delay((int)interval);
        }
    }    

}