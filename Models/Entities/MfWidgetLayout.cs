namespace ConsumerApi.Models.Entities;

public class MfWidgetLayout
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Breakpoint { get; set; } = string.Empty;
    public int W { get; set; }
    public int H { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int MinW { get; set; }
    public int MinH { get; set; }
    public int MaxW { get; set; }
    public int MaxH { get; set; }
}
