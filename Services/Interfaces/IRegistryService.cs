using ConsumerApi.Models.Responses;

namespace ConsumerApi.Services.Interfaces;

public interface IRegistryService
{
    Task<ModuleRegistryResponse> GetAccessibleModulesAsync(List<string> userPermissions, string environment);
}
