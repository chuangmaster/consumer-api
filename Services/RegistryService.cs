using ConsumerApi.Models.Responses;
using ConsumerApi.Repositories.Interfaces;
using ConsumerApi.Services.Interfaces;

namespace ConsumerApi.Services;

public class RegistryService : IRegistryService
{
    private readonly IModuleRepository _moduleRepository;

    public RegistryService(IModuleRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public async Task<ModuleRegistryResponse> GetAccessibleModulesAsync(
        List<string> userPermissions, string environment)
    {
        var allModules = await _moduleRepository.GetActiveModulesAsync(environment);

        // 以 RequiredClaim 比對使用者的 permissions，過濾可存取的模組
        var accessible = allModules
            .Where(m => userPermissions.Any(p => MatchesClaim(p, m.RequiredClaim)))
            .ToList();

        var response = new ModuleRegistryResponse();

        // 分離 widget、menu 與 overlay
        var widgets = accessible.Where(m => m.ComponentType == "widget").ToList();
        var menus = accessible.Where(m => m.ComponentType == "menu").ToList();
        var overlays = accessible.Where(m => m.ComponentType == "overlay").ToList();

        // 為 widget 載入響應式佈局配置
        if (widgets.Count > 0)
        {
            var widgetIds = widgets.Select(w => w.Id);
            var layouts = await _moduleRepository.GetWidgetLayoutsAsync(widgetIds);
            var layoutsByModule = layouts.GroupBy(l => l.ModuleId)
                .ToDictionary(g => g.Key, g => g.ToList());

            response.Widgets = widgets.Select(w => new WidgetModuleResponse
            {
                ModuleName = w.ModuleName,
                EntryUrl = w.EntryUrl,
                ExposedComponent = w.ExposedComponent,
                DisplayName = w.DisplayName,
                Layouts = layoutsByModule.TryGetValue(w.Id, out var moduleLayouts)
                    ? moduleLayouts.ToDictionary(
                        l => l.Breakpoint,
                        l => new LayoutConfig
                        {
                            W = l.W, H = l.H, X = l.X, Y = l.Y,
                            MinW = l.MinW, MinH = l.MinH,
                            MaxW = l.MaxW, MaxH = l.MaxH
                        })
                    : new Dictionary<string, LayoutConfig>()
            }).ToList();
        }

        response.Menus = menus.Select(m => new MenuModuleResponse
        {
            ModuleName = m.ModuleName,
            EntryUrl = m.EntryUrl,
            ExposedComponent = m.ExposedComponent,
            DisplayName = m.DisplayName,
            RoutePath = m.RoutePath ?? string.Empty
        }).ToList();

        response.Overlays = overlays.Select(m => new OverlayModuleResponse
        {
            ModuleName = m.ModuleName,
            EntryUrl = m.EntryUrl,
            ExposedComponent = m.ExposedComponent,
            DisplayName = m.DisplayName
        }).ToList();

        return response;
    }

    /// <summary>
    /// 判斷使用者權限是否滿足模組要求。
    /// 支援精確比對，以及 portal:admin 等級包含 portal:access 的隱含關係。
    /// 例如: 使用者有 "transport:portal:admin" → 可存取要求 "transport:portal:access" 的模組
    /// </summary>
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
