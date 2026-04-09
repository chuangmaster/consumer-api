using System.Text.Json.Serialization;

namespace ConsumerApi.Models.Responses;

public class ModuleRegistryResponse
{
    [JsonPropertyName("widgets")]
    public List<WidgetModuleResponse> Widgets { get; set; } = [];

    [JsonPropertyName("menus")]
    public List<MenuModuleResponse> Menus { get; set; } = [];
}
