using AssignmentSystem.Application.Common;
using AssignmentSystem.Application.DTOs.Assignments;
using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces
{
    public interface IAssignmentService
    {
        Task<PageResult<AssignmentDto>> GetPagedAsync(AssignmentQuery query, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
        Task<AssignmentDto> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
        Task<AssignmentDto> CreateAsync(CreateAssignmentRequest request, Guid teacherId, CancellationToken ct = default);
        Task<AssignmentDto> UpdateAsync(Guid id, UpdateAssignmentRequest request, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
        Task<AssignmentDto> PublishAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
        Task<AssignmentDto> CloseAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
        Task DeleteAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
    }
}
