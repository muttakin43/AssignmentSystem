using AssignmentSystem.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs.Submission
{
    

    public record SubmissionDto(
        Guid Id, Guid AssignmentId, string AssignmentTitle,
        Guid StudentId, string StudentName,
        string? TextAnswer, string? FileName,
        SubmissionStatus Status, decimal? MarksObtained, decimal AssignmentMaxMarks,
        string? Feedback, DateTime SubmittedAtUtc, DateTime? UpdatedAtUtc,
        DateTime? GradedAtUtc, string? GradedByTeacherName);

    public record GradeSubmissionRequest(decimal MarksObtained, string? Feedback);

    public record ChangeSubmissionStatusRequest(SubmissionStatus Status);
}
