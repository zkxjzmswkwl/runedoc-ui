using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RuneDocMVVM.Views
{
    public partial class PluginHostControl : UserControl
    {
        public PluginHostControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}