using ConsumerApi.Models.Entities;

namespace ConsumerApi.Repositories.Interfaces;

public interface IModuleRepository
{
    Task<List<MfRemoteModule>> GetActiveModulesAsync(string environment);
    Task<List<MfWidgetLayout>> GetWidgetLayoutsAsync(IEnumerable<int> moduleIds);
}
