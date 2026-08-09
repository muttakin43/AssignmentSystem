using AssignmentSystem.Application.DTOs.Classes;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AssignmentSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClassController(IClassService classService) : ControllerBase
    {
        private Guid CurrentUserId =>
         Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

        private UserRole CurrentUserRole =>
            Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await classService.GetAllAsync(CurrentUserId, CurrentUserRole, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await classService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateClassRequest request, CancellationToken ct)
        {
            var result = await classService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassRequest request, CancellationToken ct)
        {
            var result = await classService.UpdateAsync(id, request, ct);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            await classService.DeactivateAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{classId:guid}/subjects/{subjectId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LinkSubject(Guid classId, Guid subjectId, CancellationToken ct)
        {
            await classService.LinkSubjectAsync(classId, subjectId, ct);
            return NoContent();
        }

        [HttpDelete("{classId:guid}/subjects/{subjectId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnlinkSubject(Guid classId, Guid subjectId, CancellationToken ct)
        {
            await classService.UnlinkSubjectAsync(classId, subjectId, ct);
            return NoContent();
        }
    }
}
