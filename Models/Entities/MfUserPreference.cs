namespace ConsumerApi.Models.Entities;

public class MfUserPreference
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    /// <summary>JSON 字串，反序列化後為 string[]，格式: ["provider_id::ExposeName"]</summary>
    public string EnabledWidgets { get; set; } = "[]";
    /// <summary>JSON 字串，反序列化後為 LayoutItemDto[]</summary>
    public string LayoutData { get; set; } = "[]";
    /// <summary>樂觀鎖版本號，每次成功更新後遞增</summary>
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
