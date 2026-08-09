using AssignmentSystem.Application.DTOs.Classes;
using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces
{
    public interface IClassService
    {
        Task<IReadOnlyList<ClassDto>> GetAllAsync(Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);

        Task<ClassDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<ClassDto> CreateAsync(CreateClassRequest request, CancellationToken ct = default);

        Task<ClassDto> UpdateAsync(Guid id, UpdateClassRequest request, CancellationToken ct = default);

        Task DeactivateAsync(Guid id, CancellationToken ct = default);

        Task LinkSubjectAsync(Guid classId, Guid subjectId, CancellationToken ct = default);

        Task UnlinkSubjectAsync(Guid classId, Guid subjectId, CancellationToken ct = default);
    }
}
