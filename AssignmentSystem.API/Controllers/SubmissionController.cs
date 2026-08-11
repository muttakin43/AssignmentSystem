using AssignmentSystem.Application.DTOs.Submission;
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
    public class SubmissionController(ISubmissionService submissionService) : ControllerBase
    {
        private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

        private UserRole CurrentRole =>
            Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

        [HttpGet("assignments/{assignmentId:guid}/submissions")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> GetForAssignment(Guid assignmentId, CancellationToken ct)
        {
            var result = await submissionService.GetForAssignmentAsync(assignmentId, CurrentUserId, CurrentRole, ct);
            return Ok(result);
        }

        [HttpGet("submissions/mine")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMine(CancellationToken ct)
        {
            var result = await submissionService.GetMineAsync(CurrentUserId, ct);
            return Ok(result);
        }

        [HttpGet("submissions/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await submissionService.GetByIdAsync(id, CurrentUserId, CurrentRole, ct);
            return Ok(result);
        }

        [HttpPost("assignments/{assignmentId:guid}/submissions")]
        [Authorize(Roles = "Student")]
        [RequestSizeLimit(20_000_000)] 
        public async Task<IActionResult> Create(
            Guid assignmentId, [FromForm] string? textAnswer, IFormFile? file, CancellationToken ct)
        {
            var result = await submissionService.CreateAsync(assignmentId, CurrentUserId, textAnswer, file, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("submissions/{id:guid}")]
        [Authorize(Roles = "Student")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> Update(
            Guid id, [FromForm] string? textAnswer, IFormFile? file, CancellationToken ct)
        {
            var result = await submissionService.UpdateAsync(id, CurrentUserId, textAnswer, file, ct);
            return Ok(result);
        }

        [HttpPut("submissions/{id:guid}/grade")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Grade(Guid id, [FromBody] GradeSubmissionRequest request, CancellationToken ct)
        {
            var result = await submissionService.GradeAsync(id, request, CurrentUserId, CurrentRole, ct);
            return Ok(result);
        }

        [HttpPatch("submissions/{id:guid}/status")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeSubmissionStatusRequest request, CancellationToken ct)
        {
            var result = await submissionService.ChangeStatusAsync(id, request, CurrentUserId, CurrentRole, ct);
            return Ok(result);
        }

        [HttpGet("submissions/{id:guid}/file")]
        public async Task<IActionResult> GetFile(Guid id, CancellationToken ct)
        {
            var file = await submissionService.GetFileAsync(id, CurrentUserId, CurrentRole, ct);
            return File(file.Content, file.ContentType, file.FileName);
        }
    } 
}
