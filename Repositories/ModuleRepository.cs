using Dapper;
using ConsumerApi.Infrastructure;
using ConsumerApi.Models.Entities;
using ConsumerApi.Repositories.Interfaces;

namespace ConsumerApi.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly DbConnectionFactory _db;

    public ModuleRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<MfRemoteModule>> GetActiveModulesAsync(string environment)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<MfRemoteModule>(
            """
            SELECT id, module_name AS ModuleName, required_claim AS RequiredClaim,
                   entry_url AS EntryUrl, exposed_component AS ExposedComponent,
                   display_name AS DisplayName, component_type AS ComponentType,
                   route_path AS RoutePath, environment, sort_order AS SortOrder,
                   is_active AS IsActive
            FROM mf_remote_modules
            WHERE environment = @Environment AND is_active = TRUE
            ORDER BY sort_order, id
            """,
            new { Environment = environment });
        return result.ToList();
    }

    public async Task<List<MfWidgetLayout>> GetWidgetLayoutsAsync(IEnumerable<int> moduleIds)
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<MfWidgetLayout>(
            """
            SELECT id, module_id AS ModuleId, breakpoint,
                   w, h, x, y,
                   min_w AS MinW, min_h AS MinH,
                   max_w AS MaxW, max_h AS MaxH
            FROM mf_widget_layouts
            WHERE module_id = ANY(@ModuleIds)
            """,
            new { ModuleIds = moduleIds.ToArray() });
        return result.ToList();
    }
}
