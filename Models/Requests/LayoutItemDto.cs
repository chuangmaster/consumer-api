namespace ConsumerApi.Models.Requests;

public class LayoutItemDto
{
    public string I { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }
    public int MinW { get; set; }
    public int MinH { get; set; }
    public int MaxW { get; set; }
    public int MaxH { get; set; }
}
