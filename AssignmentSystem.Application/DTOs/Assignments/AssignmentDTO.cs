using AssignmentSystem.Application.Common;
using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs.Assignments
{
    public record AssignmentDto(
    Guid Id,
    string Title,
    string? Description,
    Guid ClassId,
    string ClassName,
    Guid SubjectId,
    string SubjectName,
    Guid TeacherId,
    string TeacherName,
    DateTime Deadline,
    decimal MaxMarks,
    AssignmentStatus Status,
    bool AllowUpdateAfterSubmit,
    int SubmissionCount,
    DateTime CreatedAtUtc);

    public class AssignmentQuery : PageQuery
    {
        public Guid? ClassId { get; set; }
        public Guid? SubjectId { get; set; }
        public AssignmentStatus? Status { get; set; }
        public string? Search { get; set; }
    }

    public record CreateAssignmentRequest(
        string Title,
        string? Description,
        Guid ClassId,
        Guid SubjectId,
        DateTime Deadline,
        decimal MaxMarks,
        bool AllowUpdateAfterSubmit);

    public record UpdateAssignmentRequest(
        string Title,
        string? Description,
        DateTime Deadline,
        decimal MaxMarks,
        bool AllowUpdateAfterSubmit);
}
