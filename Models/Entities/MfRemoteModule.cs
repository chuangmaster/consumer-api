namespace ConsumerApi.Models.Entities;

public class MfRemoteModule
{
    public int Id { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string RequiredClaim { get; set; } = string.Empty;
    public string EntryUrl { get; set; } = string.Empty;
    public string ExposedComponent { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ComponentType { get; set; } = "widget";
    public string? RoutePath { get; set; }
    public string Environment { get; set; } = "Development";
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
