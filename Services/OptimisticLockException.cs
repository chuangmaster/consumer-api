namespace ConsumerApi.Services;

/// <summary>
/// 當 PUT 偏好設定時，若用戶端的 version 與資料庫中的不符，拋出此例外表示樂觀鎖衝突。
/// 應對應回傳 HTTP 409 Conflict。
/// </summary>
public class OptimisticLockException : Exception
{
    public OptimisticLockException()
        : base("偏好設定已被其他請求修改，請重新取得最新版本後再試。") { }
}
