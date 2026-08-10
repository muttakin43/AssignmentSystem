using AssignmentSystem.Application.DTOs.Assignments;
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
    public class AssignmentController(IAssignmentService assignmentService) : ControllerBase
    {
        private Guid CurrentUserId =>
       Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

        private UserRole CurrentRole =>
            Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AssignmentQuery query, CancellationToken ct)
        {
            var result = await assignmentService.GetPagedAsync(query, CurrentUserId, CurrentRole, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await assignmentService.GetByIdAsync(id, CurrentUserId, CurrentRole, ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([FromBody] CreateAssignmentRequest request, CancellationToken ct)
        {
            var result = await assignmentService.CreateAsync(request, CurrentUserId, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssignmentRequest request, CancellationToken ct)
        {
            var result = await assignmentService.UpdateAsync(id, request, CurrentUserId, CurrentRole, ct);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/publish")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
        {
            var result = await assignmentService.PublishAsync(id, CurrentUserId, CurrentRole, ct);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/close")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Close(Guid id, CancellationToken ct)
        {
            var result = await assignmentService.CloseAsync(id, CurrentUserId, CurrentRole, ct);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await assignmentService.DeleteAsync(id, CurrentUserId, CurrentRole, ct);
            return NoContent();
        }
    }
}
