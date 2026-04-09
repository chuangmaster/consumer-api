using System.Text.Json.Serialization;

namespace ConsumerApi.Models.Responses;

public class LayoutConfig
{
    [JsonPropertyName("w")]
    public int W { get; set; }

    [JsonPropertyName("h")]
    public int H { get; set; }

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("minW")]
    public int MinW { get; set; }

    [JsonPropertyName("minH")]
    public int MinH { get; set; }

    [JsonPropertyName("maxW")]
    public int MaxW { get; set; }

    [JsonPropertyName("maxH")]
    public int MaxH { get; set; }
}
