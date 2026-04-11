using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ConsumerApi.Models.Requests;
using ConsumerApi.Services.Interfaces;

namespace ConsumerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementService _service;

    public AnnouncementsController(IAnnouncementService service)
    {
        _service = service;
    }

    /// <summary>
    /// 取得目前有效的公告（所有已登入使用者皆可存取）。
    /// 有效條件：is_active = TRUE AND starts_at &lt;= NOW() AND ends_at &gt; NOW()
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var result = await _service.GetActiveAsync();
        return Ok(result);
    }

    /// <summary>
    /// 取得全部公告，含過期與未啟用（需要 announcement:admin 權限）。
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        if (!HasAdminPermission())
            return Forbid();

        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// 新增公告（需要 announcement:admin 權限）。
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementRequest request)
    {
        if (!HasAdminPermission())
            return Forbid();

        var createdBy = User.FindFirst("sub")?.Value
            ?? User.Identity?.Name
            ?? "unknown";

        try
        {
            var result = await _service.CreateAsync(request, createdBy);
            return CreatedAtAction(nameof(GetActive), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 更新公告（需要 announcement:admin 權限）。
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAnnouncementRequest request)
    {
        if (!HasAdminPermission())
            return Forbid();

        try
        {
            var result = await _service.UpdateAsync(id, request);
            if (result is null) return NotFound();
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// 刪除公告（需要 announcement:admin 權限）。
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!HasAdminPermission())
            return Forbid();

        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    private bool HasAdminPermission() =>
        User.FindAll("permissions").Any(c =>
            string.Equals(c.Value, "announcement:admin", StringComparison.OrdinalIgnoreCase));
}
