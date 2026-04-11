using ConsumerApi.Models.Responses;
using ConsumerApi.Models.Requests;

namespace ConsumerApi.Services.Interfaces;

public interface IAnnouncementService
{
    Task<List<AnnouncementResponse>> GetActiveAsync();
    Task<List<AnnouncementResponse>> GetAllAsync();
    Task<AnnouncementResponse?> GetByIdAsync(int id);
    Task<AnnouncementResponse> CreateAsync(CreateAnnouncementRequest request, string createdBy);
    Task<AnnouncementResponse?> UpdateAsync(int id, UpdateAnnouncementRequest request);
    Task<bool> DeleteAsync(int id);
}
