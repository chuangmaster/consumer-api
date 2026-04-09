using ConsumerApi.Models.Requests;

namespace ConsumerApi.Models.Responses;

public class UserPreferenceResponse
{
    public string[] EnabledWidgets { get; set; } = [];
    public LayoutItemDto[] LayoutData { get; set; } = [];
    /// <summary>當前版本號，前端送 PUT 時須帶回此值以實現樂觀鎖</summary>
    public int Version { get; set; }
}
