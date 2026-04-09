using System.Text.Json;
using Dapper;
using ConsumerApi.Infrastructure;
using ConsumerApi.Models.Entities;
using ConsumerApi.Models.Requests;
using ConsumerApi.Repositories.Interfaces;

namespace ConsumerApi.Repositories;

public class PreferenceRepository : IPreferenceRepository
{
    private readonly DbConnectionFactory _db;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public PreferenceRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<MfUserPreference?> GetByUserIdAsync(string userId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<MfUserPreference>(
            """
            SELECT id, user_id AS UserId,
                   enabled_widgets::text AS EnabledWidgets,
                   layout_data::text AS LayoutData,
                   version, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM mf_user_preferences
            WHERE user_id = @UserId
            """,
            new { UserId = userId });
    }

    public async Task<MfUserPreference> CreateAsync(
        string userId, string[] enabledWidgets, LayoutItemDto[] layoutData)
    {
        using var conn = _db.CreateConnection();
        var widgetsJson = JsonSerializer.Serialize(enabledWidgets, JsonOptions);
        var layoutJson = JsonSerializer.Serialize(layoutData, JsonOptions);

        return await conn.QuerySingleAsync<MfUserPreference>(
            """
            INSERT INTO mf_user_preferences (user_id, enabled_widgets, layout_data, version)
            VALUES (@UserId, @EnabledWidgets::jsonb, @LayoutData::jsonb, 0)
            RETURNING id, user_id AS UserId,
                      enabled_widgets::text AS EnabledWidgets,
                      layout_data::text AS LayoutData,
                      version, created_at AS CreatedAt, updated_at AS UpdatedAt
            """,
            new { UserId = userId, EnabledWidgets = widgetsJson, LayoutData = layoutJson });
    }

    public async Task<bool> UpdateAsync(
        string userId, string[] enabledWidgets, LayoutItemDto[] layoutData, int expectedVersion)
    {
        using var conn = _db.CreateConnection();
        var widgetsJson = JsonSerializer.Serialize(enabledWidgets, JsonOptions);
        var layoutJson = JsonSerializer.Serialize(layoutData, JsonOptions);

        var affected = await conn.ExecuteAsync(
            """
            UPDATE mf_user_preferences
            SET enabled_widgets = @EnabledWidgets::jsonb,
                layout_data     = @LayoutData::jsonb,
                version         = version + 1,
                updated_at      = NOW()
            WHERE user_id = @UserId
              AND version  = @ExpectedVersion
            """,
            new
            {
                UserId = userId,
                EnabledWidgets = widgetsJson,
                LayoutData = layoutJson,
                ExpectedVersion = expectedVersion
            });

        return affected > 0;
    }
}
