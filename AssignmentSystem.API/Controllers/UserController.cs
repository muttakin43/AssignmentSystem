using AssignmentSystem.Application.DTOs.Users;
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
    [Authorize(Roles = "Admin")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private Guid CurrentUserId =>
       Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

        private UserRole CurrentUserRole =>
            Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserQuery query, CancellationToken ct)
        {
            var result = await userService.GetPagedAsync(query, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await userService.GetByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
        {
            var result = await userService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
        {
            var result = await userService.UpdateAsync(id, request, ct);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/active")]
        public async Task<IActionResult> SetActive(Guid id, [FromBody] bool isActive, CancellationToken ct)
        {
            var result = await userService.SetActiveAsync(id, isActive, ct);
            return Ok(result);
        }

        [HttpPut("{id:guid}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            await userService.ChangePasswordAsync(id, request, CurrentUserId, CurrentUserRole, ct);
            return NoContent();
        }
    }
}
