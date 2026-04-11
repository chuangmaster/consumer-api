using ConsumerApi.Models.Entities;
using ConsumerApi.Models.Requests;
using ConsumerApi.Models.Responses;
using ConsumerApi.Repositories.Interfaces;
using ConsumerApi.Services.Interfaces;

namespace ConsumerApi.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly IAnnouncementRepository _repository;

    public AnnouncementService(IAnnouncementRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AnnouncementResponse>> GetActiveAsync()
    {
        var items = await _repository.GetActiveAsync();
        return items.Select(ToResponse).ToList();
    }

    public async Task<List<AnnouncementResponse>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(ToResponse).ToList();
    }

    public async Task<AnnouncementResponse?> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item is null ? null : ToResponse(item);
    }

    public async Task<AnnouncementResponse> CreateAsync(CreateAnnouncementRequest request, string createdBy)
    {
        if (request.EndsAt <= request.StartsAt)
            throw new ArgumentException("ends_at 必須大於 starts_at。");

        var item = await _repository.CreateAsync(request, createdBy);
        return ToResponse(item);
    }

    public async Task<AnnouncementResponse?> UpdateAsync(int id, UpdateAnnouncementRequest request)
    {
        if (request.EndsAt <= request.StartsAt)
            throw new ArgumentException("ends_at 必須大於 starts_at。");

        var item = await _repository.UpdateAsync(id, request);
        return item is null ? null : ToResponse(item);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static AnnouncementResponse ToResponse(Announcement a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        Content = a.Content,
        StartsAt = a.StartsAt,
        EndsAt = a.EndsAt,
        IsActive = a.IsActive,
        CreatedBy = a.CreatedBy,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt,
    };
}
