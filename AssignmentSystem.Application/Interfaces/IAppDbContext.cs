using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<ClassCourse> Classes { get; }
        DbSet<Subject> Subjects { get; }
        DbSet<ClassSubject> ClassSubjects { get; }
        DbSet<TeacherAssignment> TeacherAssignments { get; }
        DbSet<Assignment> Assignments { get; }
        DbSet<Submission> Submissions { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<AppSettings> AppSettings { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
