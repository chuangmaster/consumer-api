using System.Text.Json.Serialization;

namespace ConsumerApi.Models.Requests;

/// <summary>Widget 項目，包含 Widget ID 與所屬 Section ID。</summary>
public class WidgetEntryDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("sectionId")]
    public string SectionId { get; set; } = string.Empty;
}
