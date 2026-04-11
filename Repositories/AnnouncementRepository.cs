using Dapper;
using ConsumerApi.Infrastructure;
using ConsumerApi.Models.Entities;
using ConsumerApi.Models.Requests;
using ConsumerApi.Repositories.Interfaces;

namespace ConsumerApi.Repositories;

public class AnnouncementRepository : IAnnouncementRepository
{
    private readonly DbConnectionFactory _db;

    public AnnouncementRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<Announcement>> GetActiveAsync()
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<Announcement>(
            """
            SELECT id, title, content,
                   starts_at AS StartsAt, ends_at AS EndsAt,
                   is_active AS IsActive, created_by AS CreatedBy,
                   created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM announcements
            WHERE is_active = TRUE
              AND starts_at <= NOW()
              AND ends_at > NOW()
            ORDER BY starts_at DESC
            """);
        return result.ToList();
    }

    public async Task<List<Announcement>> GetAllAsync()
    {
        using var conn = _db.CreateConnection();
        var result = await conn.QueryAsync<Announcement>(
            """
            SELECT id, title, content,
                   starts_at AS StartsAt, ends_at AS EndsAt,
                   is_active AS IsActive, created_by AS CreatedBy,
                   created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM announcements
            ORDER BY created_at DESC
            """);
        return result.ToList();
    }

    public async Task<Announcement?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Announcement>(
            """
            SELECT id, title, content,
                   starts_at AS StartsAt, ends_at AS EndsAt,
                   is_active AS IsActive, created_by AS CreatedBy,
                   created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM announcements
            WHERE id = @Id
            """,
            new { Id = id });
    }

    public async Task<Announcement> CreateAsync(CreateAnnouncementRequest request, string createdBy)
    {
        using var conn = _db.CreateConnection();
        var id = await conn.ExecuteScalarAsync<int>(
            """
            INSERT INTO announcements (title, content, starts_at, ends_at, is_active, created_by)
            VALUES (@Title, @Content, @StartsAt, @EndsAt, @IsActive, @CreatedBy)
            RETURNING id
            """,
            new
            {
                request.Title,
                request.Content,
                request.StartsAt,
                request.EndsAt,
                request.IsActive,
                CreatedBy = createdBy,
            });
        return (await GetByIdAsync(id))!;
    }

    public async Task<Announcement?> UpdateAsync(int id, UpdateAnnouncementRequest request)
    {
        using var conn = _db.CreateConnection();
        var rows = await conn.ExecuteAsync(
            """
            UPDATE announcements
            SET title = @Title,
                content = @Content,
                starts_at = @StartsAt,
                ends_at = @EndsAt,
                is_active = @IsActive,
                updated_at = NOW()
            WHERE id = @Id
            """,
            new
            {
                Id = id,
                request.Title,
                request.Content,
                request.StartsAt,
                request.EndsAt,
                request.IsActive,
            });

        if (rows == 0) return null;
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = _db.CreateConnection();
        var rows = await conn.ExecuteAsync(
            "DELETE FROM announcements WHERE id = @Id",
            new { Id = id });
        return rows > 0;
    }
}
