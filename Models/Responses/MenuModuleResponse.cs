using System.Text.Json.Serialization;

namespace ConsumerApi.Models.Responses;

public class MenuModuleResponse
{
    [JsonPropertyName("moduleName")]
    public string ModuleName { get; set; } = string.Empty;

    [JsonPropertyName("entryUrl")]
    public string EntryUrl { get; set; } = string.Empty;

    [JsonPropertyName("exposedComponent")]
    public string ExposedComponent { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("routePath")]
    public string RoutePath { get; set; } = string.Empty;
}
