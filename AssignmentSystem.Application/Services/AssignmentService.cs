using AssignmentSystem.Application.Common;
using AssignmentSystem.Application.DTOs.Assignments;
using AssignmentSystem.Application.Exceptions;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Services
{
    public class AssignmentService(IAppDbContext dbContext) : IAssignmentService
    {
        public async Task<PageResult<AssignmentDto>> GetPagedAsync(
       AssignmentQuery query, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var assignments = Query();

            switch (currentUserRole)
            {
                case UserRole.Teacher:
                    assignments = assignments.Where(a => a.TeacherId == currentUserId);
                    break;
                case UserRole.Student:
                    var studentClassId = await dbContext.Users
                        .Where(u => u.Id == currentUserId)
                        .Select(u => u.ClassId)
                        .FirstOrDefaultAsync(ct);
                    assignments = assignments.Where(a => a.ClassId == studentClassId && a.Status == AssignmentStatus.Published);
                    break;
                case UserRole.Admin:
                default:
                    break;
            }

            if (query.ClassId is not null)
                assignments = assignments.Where(a => a.ClassId == query.ClassId);

            if (query.SubjectId is not null)
                assignments = assignments.Where(a => a.SubjectId == query.SubjectId);

            if (query.Status is not null)
                assignments = assignments.Where(a => a.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                assignments = assignments.Where(a => a.Title.ToLower().Contains(search));
            }

            return await assignments
                .OrderByDescending(a => a.CreatedAtUtc)
                .Select(ToDtoExpression)
                .ToPagedResultAsync(query, ct);
        }

        public async Task<AssignmentDto> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var assignment = await Query().FirstOrDefaultAsync(a => a.Id == id, ct)
                ?? throw new NotFoundException($"Assignment '{id}' was not found.");

            if (currentUserRole == UserRole.Student)
            {
                var studentClassId = await dbContext.Users.Where(u => u.Id == currentUserId).Select(u => u.ClassId).FirstOrDefaultAsync(ct);
                var visible = assignment.Status == AssignmentStatus.Published && assignment.ClassId == studentClassId;
                if (!visible)
                    throw new NotFoundException($"Assignment '{id}' was not found.");   
            }

            if (currentUserRole == UserRole.Teacher && assignment.TeacherId != currentUserId)
                throw new ForbiddenException("You do not have access to this assignment.");

            return ToDto(assignment);
        }

        public async Task<AssignmentDto> CreateAsync(CreateAssignmentRequest request, Guid teacherId, CancellationToken ct = default)
        {
            var isAssignedTeacher = await dbContext.TeacherAssignments.AnyAsync(ta =>
                ta.TeacherId == teacherId &&
                ta.ClassId == request.ClassId &&
                ta.SubjectId == request.SubjectId &&
                ta.IsActive, ct);

            if (!isAssignedTeacher)
                throw new ForbiddenException("You are not assigned to teach this subject for this class.");

            if (request.Deadline <= DateTime.UtcNow)
                throw new ValidationException("Deadline must be in the future.");

            if (request.MaxMarks <= 0)
                throw new ValidationException("Max marks must be greater than zero.");

            var assignment = new Assignment
            {
                Title = request.Title,
                Description = request.Description,
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                TeacherId = teacherId,
                Deadline = request.Deadline,
                MaxMarks = request.MaxMarks,
                AllowUpdateAfterSubmit = request.AllowUpdateAfterSubmit,
                Status = AssignmentStatus.Draft
            };

            dbContext.Assignments.Add(assignment);
            await dbContext.SaveChangesAsync(ct);

            return await GetByIdAsync(assignment.Id, teacherId, UserRole.Teacher, ct);
        }

        public async Task<AssignmentDto> UpdateAsync(
            Guid id, UpdateAssignmentRequest request, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var assignment = await LoadOwnedAsync(id, currentUserId, currentUserRole, ct);

            if (request.Deadline <= DateTime.UtcNow)
                throw new ValidationException("Deadline must be in the future.");

            if (request.MaxMarks <= 0)
                throw new ValidationException("Max marks must be greater than zero.");

            assignment.Title = request.Title;
            assignment.Description = request.Description;
            assignment.Deadline = request.Deadline;
            assignment.MaxMarks = request.MaxMarks;
            assignment.AllowUpdateAfterSubmit = request.AllowUpdateAfterSubmit;

            await dbContext.SaveChangesAsync(ct);
            return await GetByIdAsync(id, currentUserId, currentUserRole, ct);
        }

        public async Task<AssignmentDto> PublishAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var assignment = await LoadOwnedAsync(id, currentUserId, currentUserRole, ct);

            if (assignment.Status != AssignmentStatus.Draft)
                throw new ConflictException("Only draft assignments can be published.");

            assignment.Status = AssignmentStatus.Published;
            await dbContext.SaveChangesAsync(ct);
            return await GetByIdAsync(id, currentUserId, currentUserRole, ct);
        }

        public async Task<AssignmentDto> CloseAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var assignment = await LoadOwnedAsync(id, currentUserId, currentUserRole, ct);

            if (assignment.Status != AssignmentStatus.Published)
                throw new ConflictException("Only published assignments can be closed.");

            assignment.Status = AssignmentStatus.Closed;
            await dbContext.SaveChangesAsync(ct);
            return await GetByIdAsync(id, currentUserId, currentUserRole, ct);
        }

        public async Task DeleteAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var assignment = await dbContext.Assignments
                .Include(a => a.Submissions)
                .FirstOrDefaultAsync(a => a.Id == id, ct)
                ?? throw new NotFoundException($"Assignment '{id}' was not found.");

            if (currentUserRole != UserRole.Admin && assignment.TeacherId != currentUserId)
                throw new ForbiddenException("You do not own this assignment.");

            if (assignment.Submissions.Any())
                throw new ConflictException("Cannot delete an assignment that already has submissions. Close it instead.");

            dbContext.Assignments.Remove(assignment);
            await dbContext.SaveChangesAsync(ct);
        }

        private async Task<Assignment> LoadOwnedAsync(Guid id, Guid currentUserId, UserRole currentUserRole, CancellationToken ct)
        {
            var assignment = await dbContext.Assignments.FirstOrDefaultAsync(a => a.Id == id, ct)
                ?? throw new NotFoundException($"Assignment '{id}' was not found.");

            if (currentUserRole != UserRole.Admin && assignment.TeacherId != currentUserId)
                throw new ForbiddenException("You do not own this assignment.");

            return assignment;
        }

        private IQueryable<Assignment> Query() =>
            dbContext.Assignments.Include(a => a.Class).Include(a => a.Subject).Include(a => a.Teacher).Include(a => a.Submissions);

        private static AssignmentDto ToDto(Assignment a) => new(
            a.Id, a.Title, a.Description, a.ClassId, a.Class.Name, a.SubjectId, a.Subject.Name,
            a.TeacherId, a.Teacher.FullName, a.Deadline, a.MaxMarks, a.Status, a.AllowUpdateAfterSubmit,
            a.Submissions.Count, a.CreatedAtUtc);

        private static readonly System.Linq.Expressions.Expression<Func<Assignment, AssignmentDto>> ToDtoExpression = a => new AssignmentDto(
            a.Id, a.Title, a.Description, a.ClassId, a.Class.Name, a.SubjectId, a.Subject.Name,
            a.TeacherId, a.Teacher.FullName, a.Deadline, a.MaxMarks, a.Status, a.AllowUpdateAfterSubmit,
            a.Submissions.Count, a.CreatedAtUtc);
    }
}
