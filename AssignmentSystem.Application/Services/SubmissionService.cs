using AssignmentSystem.Application.DTOs.Submission;
using AssignmentSystem.Application.Exceptions;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Services
{
    public class SubmissionService(IAppDbContext dbContext, IDateTimeProvider clock, IFileStorageService fileStorage) : ISubmissionService
    {
        public async Task<IReadOnlyList<SubmissionDto>> GetForAssignmentAsync(
       Guid assignmentId, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var assignment = await dbContext.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId, ct)
                ?? throw new NotFoundException($"Assignment '{assignmentId}' was not found.");

            if (currentUserRole != UserRole.Admin && assignment.TeacherId != currentUserId)
                throw new ForbiddenException("You do not own this assignment.");

            return await Query().Where(s => s.AssignmentId == assignmentId).Select(ToDtoExpression).ToListAsync(ct);
        }

        public async Task<IReadOnlyList<SubmissionDto>> GetMineAsync(Guid studentId, CancellationToken ct = default) =>
            await Query().Where(s => s.StudentId == studentId).OrderByDescending(s => s.SubmittedAtUtc).Select(ToDtoExpression).ToListAsync(ct);

        public async Task<SubmissionDto> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var submission = await LoadVisibleAsync(id, currentUserId, currentUserRole, ct);
            return ToDto(submission);
        }

        public async Task<SubmissionDto> CreateAsync(
            Guid assignmentId, Guid studentId, string? textAnswer, IFormFile? file, CancellationToken ct = default)
        {
            var assignment = await dbContext.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId, ct)
                ?? throw new NotFoundException($"Assignment '{assignmentId}' was not found.");

            var student = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == studentId, ct)
                ?? throw new NotFoundException("Student not found.");

            if (assignment.Status != AssignmentStatus.Published || assignment.ClassId != student.ClassId)
                throw new NotFoundException($"Assignment '{assignmentId}' was not found.");

            var alreadySubmitted = await dbContext.Submissions.AnyAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId, ct);
            if (alreadySubmitted)
                throw new ConflictException("You have already submitted this assignment. Use update instead.");

            if (string.IsNullOrWhiteSpace(textAnswer) && file is null)
                throw new BusinessRuleException("Provide a text answer, a file, or both.");

            var isLate = clock.UtcNow > assignment.Deadline;
            if (isLate && !assignment.AllowUpdateAfterSubmit)
                throw new ConflictException("The deadline for this assignment has passed.");

            var submission = new Submission
            {
                AssignmentId = assignmentId,
                StudentId = studentId,
                TextAnswer = textAnswer,
                Status = isLate ? SubmissionStatus.Late : SubmissionStatus.Submitted,
                SubmittedAtUtc = clock.UtcNow
            };

            if (file is not null)
            {
                var stored = await fileStorage.SaveAsync(file.OpenReadStream(), file.FileName, ct);
                submission.FileName = file.FileName;
                submission.FilePath = stored.RelativePath;
                submission.ContentType = file.ContentType;
                submission.FileSizeBytes = file.Length;
            }

            dbContext.Submissions.Add(submission);
            await dbContext.SaveChangesAsync(ct);

            return await GetByIdAsync(submission.Id, studentId, UserRole.Student, ct);
        }

        public async Task<SubmissionDto> UpdateAsync(Guid id, Guid studentId, string? textAnswer, IFormFile? file, CancellationToken ct = default)
        {
            var submission = await dbContext.Submissions.Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.Id == id, ct)
                ?? throw new NotFoundException($"Submission '{id}' was not found.");

            if (submission.StudentId != studentId)
                throw new ForbiddenException("You may only update your own submission.");

            if (submission.Status == SubmissionStatus.Graded)
                throw new BusinessRuleException("This submission has already been graded and can no longer be edited.");

            var assignment = submission.Assignment;
            if (assignment.Status != AssignmentStatus.Published)
                throw new ConflictException("This assignment is no longer accepting submissions.");

            var isLate = clock.UtcNow > assignment.Deadline;
            if (isLate && !assignment.AllowUpdateAfterSubmit)
                throw new ConflictException("The deadline for this assignment has passed.");

            var willHaveFile = file is not null || submission.FilePath is not null;
            if (string.IsNullOrWhiteSpace(textAnswer) && !willHaveFile)
                throw new BusinessRuleException("Provide a text answer, a file, or both.");

            submission.TextAnswer = textAnswer;
            submission.Status = isLate ? SubmissionStatus.Late : SubmissionStatus.Submitted;

            if (file is not null)
            {
                if (submission.FilePath is not null)
                    fileStorage.Delete(submission.FilePath);

                var stored = await fileStorage.SaveAsync(file.OpenReadStream(), file.FileName, ct);
                submission.FileName = file.FileName;
                submission.FilePath = stored.RelativePath;
                submission.ContentType = file.ContentType;
                submission.FileSizeBytes = file.Length;
            }

            await dbContext.SaveChangesAsync(ct);
            return await GetByIdAsync(id, studentId, UserRole.Student, ct);
        }

        public async Task<SubmissionDto> GradeAsync(
            Guid id, GradeSubmissionRequest request, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var submission = await dbContext.Submissions.Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.Id == id, ct)
                ?? throw new NotFoundException($"Submission '{id}' was not found.");

            if (currentUserRole != UserRole.Admin && submission.Assignment.TeacherId != currentUserId)
                throw new ForbiddenException("You do not own the assignment for this submission.");

            if (request.MarksObtained > submission.Assignment.MaxMarks)
                throw new BusinessRuleException($"Marks cannot exceed the maximum of {submission.Assignment.MaxMarks}.");

            submission.MarksObtained = request.MarksObtained;
            submission.Feedback = request.Feedback;
            submission.Status = SubmissionStatus.Graded;
            submission.GradedAtUtc = clock.UtcNow;
            submission.GradedByTeacherId = currentUserId;

            await dbContext.SaveChangesAsync(ct);
            return await GetByIdAsync(id, currentUserId, currentUserRole, ct);
        }

        public async Task<SubmissionDto> ChangeStatusAsync(
            Guid id, ChangeSubmissionStatusRequest request, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var submission = await dbContext.Submissions.Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.Id == id, ct)
                ?? throw new NotFoundException($"Submission '{id}' was not found.");

            if (currentUserRole != UserRole.Admin && submission.Assignment.TeacherId != currentUserId)
                throw new ForbiddenException("You do not own the assignment for this submission.");

            if (request.Status == SubmissionStatus.Graded)
                throw new BusinessRuleException("Use the grade endpoint to mark a submission as graded.");

            submission.Status = request.Status;
            await dbContext.SaveChangesAsync(ct);
            return await GetByIdAsync(id, currentUserId, currentUserRole, ct);
        }

        public async Task<SubmissionFile> GetFileAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var submission = await LoadVisibleAsync(id, currentUserId, currentUserRole, ct);

            if (submission.FilePath is null)
                throw new NotFoundException("This submission has no attached file.");

            var stream = await fileStorage.OpenReadAsync(submission.FilePath, ct);
            return new SubmissionFile(stream, submission.FileName ?? "download", submission.ContentType ?? "application/octet-stream");
        }

        private async Task<Submission> LoadVisibleAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct)
        {
            var submission = await dbContext.Submissions
                .Include(s => s.Assignment).ThenInclude(a => a.Class)
                .Include(s => s.Assignment).ThenInclude(a => a.Subject)
                .Include(s => s.Student)
                .Include(s => s.GradedByTeacher)
                .FirstOrDefaultAsync(s => s.Id == id, ct)
                ?? throw new NotFoundException($"Submission '{id}' was not found.");

            var visible = currentUserRole switch
            {
                UserRole.Admin => true,
                UserRole.Teacher => submission.Assignment.TeacherId == currentUserId,
                UserRole.Student => submission.StudentId == currentUserId,
                _ => false
            };

            if (!visible)
                throw new ForbiddenException("You do not have access to this submission.");

            return submission;
        }

        private IQueryable<Submission> Query() =>
            dbContext.Submissions.Include(s => s.Assignment).Include(s => s.Student).Include(s => s.GradedByTeacher);

        private static SubmissionDto ToDto(Submission s) => new(
            s.Id, s.AssignmentId, s.Assignment.Title, s.StudentId, s.Student.FullName, s.TextAnswer, s.FileName,
            s.Status, s.MarksObtained, s.Assignment.MaxMarks, s.Feedback, s.SubmittedAtUtc, s.UpdatedAtUtc,
            s.GradedAtUtc, s.GradedByTeacher == null ? null : s.GradedByTeacher.FullName);

        private static readonly System.Linq.Expressions.Expression<Func<Submission, SubmissionDto>> ToDtoExpression = s => new SubmissionDto(
            s.Id, s.AssignmentId, s.Assignment.Title, s.StudentId, s.Student.FullName, s.TextAnswer, s.FileName,
            s.Status, s.MarksObtained, s.Assignment.MaxMarks, s.Feedback, s.SubmittedAtUtc, s.UpdatedAtUtc,
            s.GradedAtUtc, s.GradedByTeacher == null ? null : s.GradedByTeacher.FullName);
    }
}
