using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
}