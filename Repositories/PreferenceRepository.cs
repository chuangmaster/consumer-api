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
                   sections::text AS Sections,
                   section_order::text AS SectionOrder,
                   version, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM mf_user_preferences
            WHERE user_id = @UserId
            """,
            new { UserId = userId });
    }

    public async Task<MfUserPreference> CreateAsync(
        string userId, WidgetEntryDto[] enabledWidgets, LayoutItemDto[] layoutData,
        SectionDto[]? sections = null,
        string[]? sectionOrder = null)
    {
        using var conn = _db.CreateConnection();
        var widgetsJson = JsonSerializer.Serialize(enabledWidgets, JsonOptions);
        var layoutJson = JsonSerializer.Serialize(layoutData, JsonOptions);
        var sectionsJson = JsonSerializer.Serialize(sections ?? [], JsonOptions);
        var orderJson = JsonSerializer.Serialize(sectionOrder ?? [], JsonOptions);

        return await conn.QuerySingleAsync<MfUserPreference>(
            """
            INSERT INTO mf_user_preferences
                (user_id, enabled_widgets, layout_data, sections, section_order, version)
            VALUES
                (@UserId, @EnabledWidgets::jsonb, @LayoutData::jsonb,
                 @Sections::jsonb, @SectionOrder::jsonb, 0)
            RETURNING id, user_id AS UserId,
                      enabled_widgets::text AS EnabledWidgets,
                      layout_data::text AS LayoutData,
                      sections::text AS Sections,
                      section_order::text AS SectionOrder,
                      version, created_at AS CreatedAt, updated_at AS UpdatedAt
            """,
            new
            {
                UserId = userId,
                EnabledWidgets = widgetsJson,
                LayoutData = layoutJson,
                Sections = sectionsJson,
                SectionOrder = orderJson,
            });
    }

    public async Task<bool> UpdateAsync(
        string userId, WidgetEntryDto[] enabledWidgets, LayoutItemDto[] layoutData, int expectedVersion,
        SectionDto[]? sections = null,
        string[]? sectionOrder = null)
    {
        using var conn = _db.CreateConnection();
        var widgetsJson = JsonSerializer.Serialize(enabledWidgets, JsonOptions);
        var layoutJson = JsonSerializer.Serialize(layoutData, JsonOptions);
        var sectionsJson = JsonSerializer.Serialize(sections ?? [], JsonOptions);
        var orderJson = JsonSerializer.Serialize(sectionOrder ?? [], JsonOptions);

        var affected = await conn.ExecuteAsync(
            """
            UPDATE mf_user_preferences
            SET enabled_widgets     = @EnabledWidgets::jsonb,
                layout_data         = @LayoutData::jsonb,
                sections            = @Sections::jsonb,
                section_order       = @SectionOrder::jsonb,
                version             = version + 1,
                updated_at          = NOW()
            WHERE user_id = @UserId
              AND version  = @ExpectedVersion
            """,
            new
            {
                UserId = userId,
                EnabledWidgets = widgetsJson,
                LayoutData = layoutJson,
                Sections = sectionsJson,
                SectionOrder = orderJson,
                ExpectedVersion = expectedVersion,
            });

        return affected > 0;
    }
}
