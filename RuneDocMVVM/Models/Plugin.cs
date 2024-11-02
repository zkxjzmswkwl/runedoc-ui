using System.Text.Json;

namespace RuneDocMVVM.Models;

public class PluginModel
{
    public string PluginName { get; set; }
    public string Version { get; set; }
    public JsonElement Ui { get; set; }

    public static PluginModel FromJson(string json)
    {
        return JsonSerializer.Deserialize<PluginModel>(json);
    }
}