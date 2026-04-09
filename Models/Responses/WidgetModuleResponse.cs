using System.Text.Json.Serialization;

namespace ConsumerApi.Models.Responses;

public class WidgetModuleResponse
{
    [JsonPropertyName("moduleName")]
    public string ModuleName { get; set; } = string.Empty;

    [JsonPropertyName("entryUrl")]
    public string EntryUrl { get; set; } = string.Empty;

    [JsonPropertyName("exposedComponent")]
    public string ExposedComponent { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 響應式布局配置，key 為斷點名稱 (lg/md/sm)
    /// </summary>
    [JsonPropertyName("layouts")]
    public Dictionary<string, LayoutConfig> Layouts { get; set; } = new();
}
