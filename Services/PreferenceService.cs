using System.Text.Json;
using ConsumerApi.Models.Requests;
using ConsumerApi.Models.Responses;
using ConsumerApi.Repositories.Interfaces;
using ConsumerApi.Services.Interfaces;

namespace ConsumerApi.Services;

public class PreferenceService : IPreferenceService
{
    private readonly IPreferenceRepository _preferenceRepo;
    private readonly IModuleRepository _moduleRepo;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // Widget 預設大小，與前端 useProviderStore.ts 的 WIDGET_SIZE_DEFAULTS 保持一致
    private static readonly Dictionary<string, (int W, int H, int MinW, int MinH, int MaxW, int MaxH)> WidgetSizeDefaults = new()
    {
        ["WeeklyMenu"] = (6, 6, 4, 4, 12, 10),
    };
    private static (int W, int H, int MinW, int MinH, int MaxW, int MaxH) DefaultSize => (4, 4, 2, 2, 12, 8);

    public PreferenceService(IPreferenceRepository preferenceRepo, IModuleRepository moduleRepo)
    {
        _preferenceRepo = preferenceRepo;
        _moduleRepo = moduleRepo;
    }

    public async Task<UserPreferenceResponse> GetOrCreateDefaultAsync(
        string userId, IEnumerable<string> permissions, string environment)
    {
        var existing = await _preferenceRepo.GetByUserIdAsync(userId);
        if (existing is not null)
            return MapToResponse(existing);

        // 無記錄 → 依 permissions 篩選可用 widget，建立預設偏好
        var permList = permissions.ToList();
        var allModules = await _moduleRepo.GetActiveModulesAsync(environment);
        var accessibleWidgets = allModules
            .Where(m => m.ComponentType == "widget" &&
                        permList.Any(p => MatchesClaim(p, m.RequiredClaim)))
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.Id)
            .ToList();

        // 產生預設 enabledWidgets key: "demo_provider::ExposedComponentName"
        // ExposedComponent 格式為 "./Order"，取去掉 "./" 後的部分
        var enabledWidgets = accessibleWidgets
            .Select(w => $"demo_provider::{StripPrefix(w.ExposedComponent)}")
            .ToArray();

        // 按 12 欄 grid 排列預設佈局
        var layoutData = accessibleWidgets
            .Select((w, idx) => BuildDefaultLayoutItem(
                moduleName: "remote_service",
                exposeName: StripPrefix(w.ExposedComponent),
                index: idx))
            .ToArray();

        var created = await _preferenceRepo.CreateAsync(userId, enabledWidgets, layoutData);
        return MapToResponse(created);
    }

    public async Task<UserPreferenceResponse> SaveAsync(string userId, SavePreferenceRequest request)
    {
        var existing = await _preferenceRepo.GetByUserIdAsync(userId);

        if (existing is null)
        {
            // 首次儲存 → 直接建立（忽略前端傳來的 version，初始版本即為 0）
            var created = await _preferenceRepo.CreateAsync(
                userId, request.EnabledWidgets, request.LayoutData);
            return MapToResponse(created);
        }

        var updated = await _preferenceRepo.UpdateAsync(
            userId, request.EnabledWidgets, request.LayoutData, request.Version);

        if (!updated)
            throw new OptimisticLockException();

        // 回傳更新後的版本（version + 1）
        var latest = await _preferenceRepo.GetByUserIdAsync(userId);
        return MapToResponse(latest!);
    }

    // ── Helpers ──

    private static UserPreferenceResponse MapToResponse(Models.Entities.MfUserPreference entity)
    {
        var widgets = JsonSerializer.Deserialize<string[]>(entity.EnabledWidgets, JsonOptions) ?? [];
        var layout = JsonSerializer.Deserialize<LayoutItemDto[]>(entity.LayoutData, JsonOptions) ?? [];
        return new UserPreferenceResponse
        {
            EnabledWidgets = widgets,
            LayoutData = layout,
            Version = entity.Version
        };
    }

    private static LayoutItemDto BuildDefaultLayoutItem(string moduleName, string exposeName, int index)
    {
        var size = WidgetSizeDefaults.TryGetValue(exposeName, out var s) ? s : DefaultSize;
        const int colNum = 12;
        var x = (index * size.W) % colNum;
        var y = (int)Math.Floor((double)(index * size.W) / colNum) * size.H;
        return new LayoutItemDto
        {
            I = $"{moduleName}/{exposeName}",
            X = x, Y = y,
            W = size.W, H = size.H,
            MinW = size.MinW, MinH = size.MinH,
            MaxW = size.MaxW, MaxH = size.MaxH
        };
    }

    /// <summary>將 "./Order" 轉換為 "Order"</summary>
    private static string StripPrefix(string exposedComponent) =>
        exposedComponent.TrimStart('.', '/');

    private static bool MatchesClaim(string userPermission, string requiredClaim)
    {
        if (string.Equals(userPermission, requiredClaim, StringComparison.OrdinalIgnoreCase))
            return true;

        // portal:admin 隱含 portal:access
        if (requiredClaim.EndsWith(":access", StringComparison.OrdinalIgnoreCase))
        {
            var prefix = requiredClaim[..requiredClaim.LastIndexOf(':')];
            if (userPermission.StartsWith(prefix + ":", StringComparison.OrdinalIgnoreCase)
                && userPermission.EndsWith(":admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
