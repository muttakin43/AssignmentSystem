using AssignmentSystem.Application.DTOs.AppSettings;
using AssignmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class AppSettingsController(IAppSettingsService settingService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await settingService.GetAllAsync(ct);
            return Ok(result);
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetByKey(string key, CancellationToken ct)
        {
            var result = await settingService.GetByKeyAsync(key, ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppSettingRequest request, CancellationToken ct)
        {
            var result = await settingService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetByKey), new { key = result.Key }, result);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] UpdateAppSettingRequest request, CancellationToken ct)
        {
            var result = await settingService.UpdateAsync(key, request, ct);
            return Ok(result);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key, CancellationToken ct)
        {
            await settingService.DeleteAsync(key, ct);
            return NoContent();
        }
    }
}
