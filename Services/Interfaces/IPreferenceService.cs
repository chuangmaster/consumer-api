using ConsumerApi.Models.Requests;
using ConsumerApi.Models.Responses;

namespace ConsumerApi.Services.Interfaces;

public interface IPreferenceService
{
    /// <summary>
    /// 取得用戶偏好設定。若尚無記錄，依 JWT permissions 自動建立預設值。
    /// </summary>
    Task<UserPreferenceResponse> GetOrCreateDefaultAsync(string userId, IEnumerable<string> permissions, string environment);

    /// <summary>
    /// 儲存用戶偏好設定（含樂觀鎖）。
    /// </summary>
    /// <exception cref="OptimisticLockException">版本衝突時拋出</exception>
    Task<UserPreferenceResponse> SaveAsync(string userId, SavePreferenceRequest request);
}
