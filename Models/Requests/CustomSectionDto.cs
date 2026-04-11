using System.Text.Json.Serialization;

namespace ConsumerApi.Models.Requests;

/// <summary>統一的 Section 定義。Provider 改名覆蓋只含 Id/Title；自訂 Section 額外含 IsCustom=true。</summary>
public class SectionDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("isCustom")]
    public bool? IsCustom { get; set; }
}
