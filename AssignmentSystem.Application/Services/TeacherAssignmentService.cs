using AssignmentSystem.Application.DTOs.TeacherAssignments;
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
    public class TeacherAssignmentService(IAppDbContext dbContext) : ITeacherAssignmentService
    {
        public async Task<IReadOnlyList<TeacherAssignmentDto>> GetAllAsync(CancellationToken ct = default) =>
         await Query().OrderBy(ta => ta.Class.Name).ThenBy(ta => ta.Subject.Name).Select(ToDtoExpression).ToListAsync(ct);

        public async Task<IReadOnlyList<TeacherAssignmentDto>> GetMineAsync(Guid teacherId, CancellationToken ct = default) =>
            await Query()
                .Where(ta => ta.TeacherId == teacherId && ta.IsActive)
                .OrderBy(ta => ta.Class.Name)
                .Select(ToDtoExpression)
                .ToListAsync(ct);

        public async Task<TeacherAssignmentDto> CreateAsync(CreateTeacherAssignmentRequest request, CancellationToken ct = default)
        {
            var teacher = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.TeacherId, ct)
                ?? throw new NotFoundException($"Teacher '{request.TeacherId}' was not found.");

            if (teacher.Role != UserRole.Teacher)
            {
                throw new BusinessRuleException("The specified user is not a Teacher.");
            }

            var classExists = await dbContext.Classes.AnyAsync(c => c.Id == request.ClassId, ct);
            if (!classExists)
            {
                throw new NotFoundException($"Class '{request.ClassId}' was not found.");
            }

            var subjectExists = await dbContext.Subjects.AnyAsync(s => s.Id == request.SubjectId, ct);
            if (!subjectExists)
            {
                throw new NotFoundException($"Subject '{request.SubjectId}' was not found.");
            }

            var conflict = await dbContext.TeacherAssignments.AnyAsync(
                ta => ta.ClassId == request.ClassId && ta.SubjectId == request.SubjectId && ta.IsActive, ct);
            if (conflict)
            {
                throw new ConflictException("Another teacher is already assigned to this subject for this class.");
            }

            var assignment = new TeacherAssignment
            {
                TeacherId = request.TeacherId,
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                IsActive = true
            };

            dbContext.TeacherAssignments.Add(assignment);
            await dbContext.SaveChangesAsync(ct);

            var dto = await Query().Where(ta => ta.Id == assignment.Id).Select(ToDtoExpression).FirstAsync(ct);
            return dto;
        }

        public async Task<TeacherAssignmentDto> SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
        {
            var assignment = await dbContext.TeacherAssignments.FirstOrDefaultAsync(ta => ta.Id == id, ct)
                ?? throw new NotFoundException($"Teacher assignment '{id}' was not found.");

            
            if (isActive && !assignment.IsActive)
            {
                var conflict = await dbContext.TeacherAssignments.AnyAsync(ta =>
                    ta.Id != id &&
                    ta.ClassId == assignment.ClassId &&
                    ta.SubjectId == assignment.SubjectId &&
                    ta.IsActive, ct);

                if (conflict)
                    throw new ConflictException("Another teacher is already actively assigned to this subject for this class.");
            }

            assignment.IsActive = isActive;
            await dbContext.SaveChangesAsync(ct);

            var dto = await Query().Where(ta => ta.Id == id).Select(ToDtoExpression).FirstAsync(ct);
            return dto;
        }

        private IQueryable<TeacherAssignment> Query() =>
            dbContext.TeacherAssignments.Include(ta => ta.Teacher).Include(ta => ta.Class).Include(ta => ta.Subject);

        private static readonly System.Linq.Expressions.Expression<Func<TeacherAssignment, TeacherAssignmentDto>> ToDtoExpression =
            ta => new TeacherAssignmentDto(
                ta.Id, ta.TeacherId, ta.Teacher.FullName, ta.ClassId, ta.Class.Name, ta.SubjectId, ta.Subject.Name, ta.IsActive);
    }
}
