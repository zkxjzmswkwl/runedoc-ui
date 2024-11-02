using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using RuneDocMVVM.ViewModels;
using RuneDocMVVM.Models;

namespace RuneDocMVVM;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        string? name = data switch
        {
            ViewModelBase vm when vm.GetType().Name == "PluginHostViewModel" =>
                "RuneDocMVVM.Views.PluginHostControl",
            ViewModelBase =>
                data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal),
            PluginModel =>
                "RuneDocMVVM.Views.PluginView",
            _ =>
                data.GetType().FullName!.Replace("Model", "View", StringComparison.Ordinal)
        };

        var type = Type.GetType(name);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }
    
    public bool Match(object? data)
    {
        return data is ViewModelBase || data is PluginModel;
    }
}