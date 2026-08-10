using AssignmentSystem.Application.DTOs.Submission;
using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AssignmentSystem.Application.Interfaces
{
    public record SubmissionFile(Stream Content, string FileName, string ContentType);

    public interface ISubmissionService
    {
        Task<IReadOnlyList<SubmissionDto>> GetForAssignmentAsync(Guid assignmentId, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
        Task<IReadOnlyList<SubmissionDto>> GetMineAsync(Guid studentId, CancellationToken ct = default);
        Task<SubmissionDto> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
        Task<SubmissionDto> CreateAsync(Guid assignmentId, Guid studentId, string? textAnswer, IFormFile? file, CancellationToken ct = default);
        Task<SubmissionDto> UpdateAsync(Guid id, Guid studentId, string? textAnswer, IFormFile? file, CancellationToken ct = default);
        Task<SubmissionDto> GradeAsync(Guid id, GradeSubmissionRequest request, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
        Task<SubmissionDto> ChangeStatusAsync(Guid id, ChangeSubmissionStatusRequest request, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
        Task<SubmissionFile> GetFileAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
    }

}
