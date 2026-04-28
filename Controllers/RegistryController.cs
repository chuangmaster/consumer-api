using ConsumerApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsumerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegistryController : ControllerBase
{
    private readonly IRegistryService _registryService;
    private readonly IConfiguration _configuration;

    public RegistryController(IRegistryService registryService, IConfiguration configuration)
    {
        _registryService = registryService;
        _configuration = configuration;
    }

    /// <summary>
    /// 取得目前使用者可存取的模組列表，依 ComponentType 分為 widgets 和 menus。
    /// Widget 類型附帶 lg/md/sm 三組響應式 grid layout 配置。
    /// </summary>
    [HttpGet("modules")]
    public async Task<IActionResult> GetModules()
    {
        var permissions = User.FindAll("permissions").Select(c => c.Value).ToList();
        var environment = _configuration["ModuleEnvironment"] ?? "Development";

        var result = await _registryService.GetAccessibleModulesAsync(permissions, environment);
        return Ok(result);
    }
}
