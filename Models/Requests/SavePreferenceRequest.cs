using System.ComponentModel.DataAnnotations;

namespace ConsumerApi.Models.Requests;

public class SavePreferenceRequest
{
    [Required]
    public string[] EnabledWidgets { get; set; } = [];

    [Required]
    public LayoutItemDto[] LayoutData { get; set; } = [];

    /// <summary>
    /// 樂觀鎖版本號。必須與資料庫中的當前版本一致，否則回傳 409 Conflict。
    /// </summary>
    [Required]
    public int Version { get; set; }
}
