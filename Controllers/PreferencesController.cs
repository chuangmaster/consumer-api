using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ConsumerApi.Models.Requests;
using ConsumerApi.Services;
using ConsumerApi.Services.Interfaces;

namespace ConsumerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PreferencesController : ControllerBase
{
    private readonly IPreferenceService _preferenceService;
    private readonly IConfiguration _configuration;

    public PreferencesController(IPreferenceService preferenceService, IConfiguration configuration)
    {
        _preferenceService = preferenceService;
        _configuration = configuration;
    }

    /// <summary>
    /// 取得目前使用者的偏好設定（widget 開關 + 佈局）。
    /// 若尚無記錄，依 JWT permissions 自動建立預設值後回傳。
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        var permissions = User.FindAll("permissions").Select(c => c.Value);
        var environment = _configuration["ModuleEnvironment"] ?? "Development";
        var result = await _preferenceService.GetOrCreateDefaultAsync(userId, permissions, environment);
        return Ok(result);
    }

    /// <summary>
    /// 儲存目前使用者的偏好設定（含樂觀鎖）。
    /// 若 version 不符合資料庫中的版本，回傳 409 Conflict，前端應重新 GET 後再試。
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] SavePreferenceRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _preferenceService.SaveAsync(userId, request);
            return Ok(result);
        }
        catch (OptimisticLockException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private string? GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");
}
