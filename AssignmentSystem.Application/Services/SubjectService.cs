using AssignmentSystem.Application.DTOs.Subjects;
using AssignmentSystem.Application.Exceptions;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Services
{
    internal class SubjectService(IAppDbContext dbContext) : ISubjectService
    {
        public async Task<IReadOnlyList<SubjectDto>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.Subjects
            .OrderBy(s => s.Name)
            .Select(s => new SubjectDto(s.Id, s.Name, s.Code, s.IsActive))
            .ToListAsync(ct);

        public async Task<SubjectDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var subject = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == id, ct)
                ?? throw new NotFoundException($"Subject '{id}' was not found.");

            return new SubjectDto(subject.Id, subject.Name, subject.Code, subject.IsActive);
        }

        public async Task<SubjectDto> CreateAsync(CreateSubjectRequest request, CancellationToken ct = default)
        {
            var codeTaken = await dbContext.Subjects.AnyAsync(s => s.Code == request.Code, ct);
            if (codeTaken)
            {
                throw new ConflictException($"A subject with code '{request.Code}' already exists.");
            }

            var subject = new Subject { Name = request.Name, Code = request.Code };
            dbContext.Subjects.Add(subject);
            await dbContext.SaveChangesAsync(ct);

            return new SubjectDto(subject.Id, subject.Name, subject.Code, subject.IsActive);
        }

        public async Task<SubjectDto> UpdateAsync(Guid id, UpdateSubjectRequest request, CancellationToken ct = default)
        {
            var subject = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == id, ct)
                ?? throw new NotFoundException($"Subject '{id}' was not found.");

            var codeTaken = await dbContext.Subjects.AnyAsync(s => s.Code == request.Code && s.Id != id, ct);
            if (codeTaken)
            {
                throw new ConflictException($"A subject with code '{request.Code}' already exists.");
            }

            subject.Name = request.Name;
            subject.Code = request.Code;
            subject.IsActive = request.IsActive;

            await dbContext.SaveChangesAsync(ct);
            return new SubjectDto(subject.Id, subject.Name, subject.Code, subject.IsActive);
        }

        public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            var subject = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Id == id, ct)
                ?? throw new NotFoundException($"Subject '{id}' was not found.");

            subject.IsActive = false;
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
