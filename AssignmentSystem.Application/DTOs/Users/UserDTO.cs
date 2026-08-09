using AssignmentSystem.Application.Common;
using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs.Users
{
    public record UserDto(
    Guid Id,
    string FullName,
    string Email,
    UserRole Role,
    Guid? ClassId,
    string? ClassName,
    bool IsActive,
    DateTime CreatedAtUtc);

    public class UserQuery : PageQuery
    {
        public UserRole? Role { get; set; }
        public Guid? ClassId { get; set; }
        public string? Search { get; set; }
    }

    public record CreateUserRequest(string FullName, string Email, string Password, UserRole Role, Guid? ClassId);

    public record UpdateUserRequest(string FullName, string Email, UserRole Role, Guid? ClassId);

    public record ChangePasswordRequest(string NewPassword);
}
