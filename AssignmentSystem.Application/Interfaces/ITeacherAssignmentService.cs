using AssignmentSystem.Application.DTOs.TeacherAssignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces
{
    public interface ITeacherAssignmentService
    {
        Task<IReadOnlyList<TeacherAssignmentDto>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TeacherAssignmentDto>> GetMineAsync(Guid teacherId, CancellationToken ct = default);
        Task<TeacherAssignmentDto> CreateAsync(CreateTeacherAssignmentRequest request, CancellationToken ct = default);
        Task<TeacherAssignmentDto> SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default);
    }
}
