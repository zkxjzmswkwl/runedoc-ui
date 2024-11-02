using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RuneDocMVVM.Models;
using System;
using System.Linq;
using System.Text.Json;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;

namespace RuneDocMVVM.Views
{
    public partial class PluginView : UserControl
    {
        public PluginView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            if (DataContext is PluginModel plugin)
            {
                GeneratePluginUI(plugin);
            }
        }

        private void GeneratePluginUI(PluginModel plugin)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var uiElement = CreateLayout(plugin.Ui);
                var pluginContent = this.FindControl<ContentControl>("PluginContent");
                pluginContent.Content = uiElement;
            });
        }

        private Control CreateLayout(JsonElement uiDef)
        {
            Control layoutControl = null;
            if (uiDef.TryGetProperty("layout", out JsonElement layoutElement))
            {
                string layoutType = layoutElement.GetString();
                switch (layoutType)
                {
                    case "StackPanel":
                        var stackPanel = new StackPanel
                        {
                            Orientation = uiDef.TryGetProperty("orientation", out JsonElement orientationElement) &&
                                          orientationElement.GetString().Equals("Horizontal",
                                              StringComparison.OrdinalIgnoreCase)
                                ? Orientation.Horizontal
                                : Orientation.Vertical
                        };
                        if (uiDef.TryGetProperty("children", out JsonElement childrenElement) &&
                            childrenElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var childElement in childrenElement.EnumerateArray())
                            {
                                var childControl = CreateControl(childElement);
                                if (childControl != null)
                                {
                                    stackPanel.Children.Add(childControl);
                                }
                            }
                        }

                        layoutControl = stackPanel;
                        break;
                    case "Grid":
                        var grid = new Grid();
                        int columns = uiDef.TryGetProperty("columns", out JsonElement columnsElement) &&
                                      columnsElement.TryGetInt32(out int col)
                            ? col
                            : 1;
                        for (int i = 0; i < columns; i++)
                        {
                            grid.ColumnDefinitions.Add(new ColumnDefinition());
                        }

                        if (uiDef.TryGetProperty("children", out JsonElement gridChildrenElement) &&
                            gridChildrenElement.ValueKind == JsonValueKind.Array)
                        {
                            var childrenArray = gridChildrenElement.EnumerateArray();
                            int totalChildren = childrenArray.Count();
                            int rows = (totalChildren + columns - 1) / columns;
                            for (int i = 0; i < rows; i++)
                            {
                                grid.RowDefinitions.Add(new RowDefinition());
                            }

                            int currentChild = 0;
                            foreach (var childElement in childrenArray)
                            {
                                var childControl = CreateControl(childElement);
                                if (childControl != null)
                                {
                                    grid.Children.Add(childControl);
                                    Grid.SetColumn(childControl, currentChild % columns);
                                    Grid.SetRow(childControl, currentChild / columns);
                                    currentChild++;
                                }
                            }
                        }

                        layoutControl = grid;
                        break;
                    default:
                        // defaults to stackpanel
                        layoutControl = new StackPanel();
                        break;
                }
            }
            else
            {
                // can specify nothing, still stackpanel.
                layoutControl = new StackPanel();
            }

            return layoutControl;
        }

        private Control CreateControl(JsonElement controlDef)
        {
            if (!controlDef.TryGetProperty("type", out JsonElement typeElement))
            {
                return null;
            }

            string controlType = typeElement.GetString();
            Control control = null;
            switch (controlType)
            {
                case "TextBlock":
                    control = new TextBlock();
                    break;
                case "Button":
                    control = new Button();
                    break;
                case "ProgressBar":
                    control = new ProgressBar();
                    break;
                case "TextBox":
                    control = new TextBox();
                    break;
                case "CheckBox":
                    control = new CheckBox();
                    break;
                default:
                    control = new TextBlock { Text = $"Unknown control type: {controlType}" };
                    break;
            }

            if (controlDef.TryGetProperty("properties", out JsonElement propertiesElement))
            {
                foreach (var prop in propertiesElement.EnumerateObject())
                {
                    SetControlProperty(control, prop.Name, prop.Value);
                }
            }

            return control;
        }

        private void SetControlProperty(Control control, string propertyName, JsonElement value)
        {
            if (propertyName == "Command" && value.ValueKind == JsonValueKind.String)
            {
                string commandName = value.GetString();
                if (control is Button button)
                {
                    button.Command = new RelayCommand(() => ExecuteCommand(commandName));
                }
            }
            else
            {
                var propInfo = control.GetType().GetProperty(propertyName);

                if (propInfo != null && propInfo.CanWrite)
                {
                    try
                    {
                        object convertedValue = null;

                        // Handle different property types
                        if (propInfo.PropertyType == typeof(string))
                        {
                            convertedValue = value.GetString();
                        }
                        else if (propInfo.PropertyType == typeof(int))
                        {
                            if (value.TryGetInt32(out int intValue))
                            {
                                convertedValue = intValue;
                            }
                        }
                        else if (propInfo.PropertyType == typeof(double))
                        {
                            if (value.TryGetDouble(out double doubleValue))
                            {
                                convertedValue = doubleValue;
                            }
                        }
                        else if (propInfo.PropertyType == typeof(bool))
                        {
                            if (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
                            {
                                convertedValue = value.GetBoolean();
                            }
                        }
                        else if (propInfo.PropertyType.IsEnum)
                        {
                            string enumValue = value.GetString();
                            convertedValue = Enum.Parse(propInfo.PropertyType, enumValue);
                        }
                        else if (propInfo.PropertyType == typeof(IBrush))
                        {
                            string colorStr = value.GetString();
                            convertedValue = new SolidColorBrush(Color.Parse(colorStr));
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(value.ToString(), propInfo.PropertyType);
                        }

                        propInfo.SetValue(control, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error setting property '{propertyName}' on {control.GetType().Name}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Property '{propertyName}' not found on control of type '{control.GetType().Name}'.");
                }
            }
        }


        private void ExecuteCommand(string commandName)
        {
            Console.WriteLine($"Command executed: {commandName}");
        }
    }
}