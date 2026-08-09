using AssignmentSystem.Application.DTOs.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces
{
    public interface ISubjectService
    {
        Task<IReadOnlyList<SubjectDto>> GetAllAsync(CancellationToken ct = default);
        Task<SubjectDto> GetByIdAsync(Guid id, CancellationToken ct = default);   
        Task<SubjectDto> CreateAsync(CreateSubjectRequest request, CancellationToken ct = default);
        Task<SubjectDto> UpdateAsync(Guid id, UpdateSubjectRequest request, CancellationToken ct = default);
        Task DeactivateAsync(Guid id, CancellationToken ct = default);
    }
}
