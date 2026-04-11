using ConsumerApi.Models.Entities;
using ConsumerApi.Models.Requests;

namespace ConsumerApi.Repositories.Interfaces;

public interface IAnnouncementRepository
{
    Task<List<Announcement>> GetActiveAsync();
    Task<List<Announcement>> GetAllAsync();
    Task<Announcement?> GetByIdAsync(int id);
    Task<Announcement> CreateAsync(CreateAnnouncementRequest request, string createdBy);
    Task<Announcement?> UpdateAsync(int id, UpdateAnnouncementRequest request);
    Task<bool> DeleteAsync(int id);
}
