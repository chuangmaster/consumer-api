using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsumerApi.Models.Requests;

public class SavePreferenceRequest
{
    [Required]
    [JsonPropertyName("enabled_widgets")]
    public WidgetEntryDto[] EnabledWidgets { get; set; } = [];

    [Required]
    [JsonPropertyName("layout_data")]
    public LayoutItemDto[] LayoutData { get; set; } = [];

    [JsonPropertyName("sections")]
    public SectionDto[] Sections { get; set; } = [];

    [JsonPropertyName("section_order")]
    public string[] SectionOrder { get; set; } = [];

    /// <summary>
    /// 樂觀鎖版本號。必須與資料庫中的當前版本一致，否則回傳 409 Conflict。
    /// </summary>
    [Required]
    [JsonPropertyName("version")]
    public int Version { get; set; }
}
