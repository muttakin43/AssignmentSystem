using AssignmentSystem.Application.Common;
using AssignmentSystem.Application.DTOs.Users;
using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces
{
    public interface IUserService
    {
        Task<PageResult<UserDto>> GetPagedAsync(UserQuery query, CancellationToken ct = default);

        Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default);

        Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);

        Task<UserDto> SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default);

        Task ChangePasswordAsync(Guid id, ChangePasswordRequest request, Guid requestingUserId, UserRole requestingUserRole, CancellationToken ct = default);
    }
}
