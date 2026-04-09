namespace ConsumerApi.Models.Responses;

public class WidgetModuleResponse
{
    public string ModuleName { get; set; } = string.Empty;

    public string EntryUrl { get; set; } = string.Empty;

    public string ExposedComponent { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 響應式布局配置，key 為斷點名稱 (lg/md/sm)
    /// </summary>
    public Dictionary<string, LayoutConfig> Layouts { get; set; } = new();
}
