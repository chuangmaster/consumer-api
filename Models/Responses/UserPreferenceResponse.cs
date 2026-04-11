using System.Text.Json.Serialization;
using ConsumerApi.Models.Requests;

namespace ConsumerApi.Models.Responses;

public class UserPreferenceResponse
{
    [JsonPropertyName("enabled_widgets")]
    public WidgetEntryDto[] EnabledWidgets { get; set; } = [];
    [JsonPropertyName("layout_data")]
    public LayoutItemDto[] LayoutData { get; set; } = [];
    [JsonPropertyName("sections")]
    public SectionDto[] Sections { get; set; } = [];
    [JsonPropertyName("section_order")]
    public string[] SectionOrder { get; set; } = [];
    /// <summary>當前版本號，前端送 PUT 時須帶回此值以實現樂觀鎖</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }
}
