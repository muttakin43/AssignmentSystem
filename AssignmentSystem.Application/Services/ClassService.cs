using AssignmentSystem.Application.DTOs.Classes;
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
    public class ClassService(IAppDbContext dbContext) : IClassService
    {
        public async Task<IReadOnlyList<ClassDto>> GetAllAsync(Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default)
        {
            var classes = dbContext.Classes.AsQueryable();

            if (currentUserRole == UserRole.Student)
            {
                classes = classes.Where(c => c.Students.Any(s => s.Id == currentUserId));
            }

            return await classes
                .OrderBy(c => c.Name)
                .Select(c => new ClassDto(c.Id, c.Name, c.Description, c.IsActive, c.Students.Count))
                .ToListAsync(ct);
        }

        public async Task<ClassDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var classCourse = await dbContext.Classes
                .Include(c => c.ClassSubjects).ThenInclude(cs => cs.Subject)
                .FirstOrDefaultAsync(c => c.Id == id, ct)
                ?? throw new NotFoundException($"Class '{id}' was not found.");

            var studentCount = await dbContext.Users.CountAsync(u => u.ClassId == id, ct);

            return new ClassDetailDto(
                classCourse.Id,
                classCourse.Name,
                classCourse.Description,
                classCourse.IsActive,
                studentCount,
                classCourse.ClassSubjects
                    .Select(cs => new ClassSubjectDto(cs.SubjectId, cs.Subject.Name, cs.Subject.Code))
                    .ToList());
        }

        public async Task<ClassDto> CreateAsync(CreateClassRequest request, CancellationToken ct = default)
        {
            var classCourse = new ClassCourse { Name = request.Name, Description = request.Description };
            dbContext.Classes.Add(classCourse);
            await dbContext.SaveChangesAsync(ct);

            return new ClassDto(classCourse.Id, classCourse.Name, classCourse.Description, classCourse.IsActive, 0);
        }

        public async Task<ClassDto> UpdateAsync(Guid id, UpdateClassRequest request, CancellationToken ct = default)
        {
            var classCourse = await dbContext.Classes.FirstOrDefaultAsync(c => c.Id == id, ct)
                ?? throw new NotFoundException($"Class '{id}' was not found.");

            classCourse.Name = request.Name;
            classCourse.Description = request.Description;
            classCourse.IsActive = request.IsActive;

            await dbContext.SaveChangesAsync(ct);

            var studentCount = await dbContext.Users.CountAsync(u => u.ClassId == id, ct);
            return new ClassDto(classCourse.Id, classCourse.Name, classCourse.Description, classCourse.IsActive, studentCount);
        }

        public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            var classCourse = await dbContext.Classes.FirstOrDefaultAsync(c => c.Id == id, ct)
                ?? throw new NotFoundException($"Class '{id}' was not found.");

            classCourse.IsActive = false;
            await dbContext.SaveChangesAsync(ct);
        }

        public async Task LinkSubjectAsync(Guid classId, Guid subjectId, CancellationToken ct = default)
        {
            var classExists = await dbContext.Classes.AnyAsync(c => c.Id == classId, ct);
            if (!classExists)
            {
                throw new NotFoundException($"Class '{classId}' was not found.");
            }

            var subjectExists = await dbContext.Subjects.AnyAsync(s => s.Id == subjectId, ct);
            if (!subjectExists)
            {
                throw new NotFoundException($"Subject '{subjectId}' was not found.");
            }

            var alreadyLinked = await dbContext.ClassSubjects.AnyAsync(cs => cs.ClassId == classId && cs.SubjectId == subjectId, ct);
            if (alreadyLinked)
            {
                throw new ConflictException("This subject is already linked to this class.");
            }

            dbContext.ClassSubjects.Add(new ClassSubject { ClassId = classId, SubjectId = subjectId });
            await dbContext.SaveChangesAsync(ct);
        }

        public async Task UnlinkSubjectAsync(Guid classId, Guid subjectId, CancellationToken ct = default)
        {
            var link = await dbContext.ClassSubjects.FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.SubjectId == subjectId, ct)
                ?? throw new NotFoundException("This subject is not linked to this class.");

            dbContext.ClassSubjects.Remove(link);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
