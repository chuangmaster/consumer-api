using ConsumerApi.Models.Entities;
using ConsumerApi.Models.Requests;

namespace ConsumerApi.Repositories.Interfaces;

public interface IPreferenceRepository
{
    Task<MfUserPreference?> GetByUserIdAsync(string userId);
    Task<MfUserPreference> CreateAsync(
        string userId,
        WidgetEntryDto[] enabledWidgets,
        LayoutItemDto[] layoutData,
        SectionDto[]? sections = null,
        string[]? sectionOrder = null);
    /// <summary>
    /// 使用樂觀鎖更新偏好設定。
    /// </summary>
    /// <returns>true 代表更新成功；false 代表版本不符（已被其他請求修改），應回傳 409</returns>
    Task<bool> UpdateAsync(
        string userId,
        WidgetEntryDto[] enabledWidgets,
        LayoutItemDto[] layoutData,
        int expectedVersion,
        SectionDto[]? sections = null,
        string[]? sectionOrder = null);
}
