using AssignmentSystem.Application.DTOs.TeacherAssignments;
using AssignmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AssignmentSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeacherAssignmentController(ITeacherAssignmentService service) : ControllerBase
    {
        private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await service.GetAllAsync(ct);
            return Ok(result);
        }

        [HttpGet("mine")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMine(CancellationToken ct)
        {
            var result = await service.GetMineAsync(CurrentUserId, ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateTeacherAssignmentRequest request, CancellationToken ct)
        {
            var result = await service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetAll), result);
        }

        [HttpPatch("{id:guid}/active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetActive(Guid id, [FromBody] bool isActive, CancellationToken ct)
        {
            var result = await service.SetActiveAsync(id, isActive, ct);
            return Ok(result);
        }
    }
}
