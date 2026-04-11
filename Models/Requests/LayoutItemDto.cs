using System.Text.Json.Serialization;

namespace ConsumerApi.Models.Requests;

public class LayoutItemDto
{
    [JsonPropertyName("i")]
    public string I { get; set; } = string.Empty;
    [JsonPropertyName("x")]
    public int X { get; set; }
    [JsonPropertyName("y")]
    public int Y { get; set; }
    [JsonPropertyName("w")]
    public int W { get; set; }
    [JsonPropertyName("h")]
    public int H { get; set; }
    [JsonPropertyName("min_w")]
    public int MinW { get; set; }
    [JsonPropertyName("min_h")]
    public int MinH { get; set; }
    [JsonPropertyName("max_w")]
    public int MaxW { get; set; }
    [JsonPropertyName("max_h")]
    public int MaxH { get; set; }
}
