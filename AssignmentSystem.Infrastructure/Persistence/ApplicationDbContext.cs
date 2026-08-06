using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Common;
using AssignmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<ClassCourse> Classes => Set<ClassCourse>();

        public DbSet<Subject> Subjects => Set<Subject>();

        public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();

        public DbSet<TeacherAssignment> TeacherAssignments => Set<TeacherAssignment>();

        public DbSet<Assignment> Assignments => Set<Assignment>();

        public DbSet<Submission> Submissions => Set<Submission>();

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<AppSettings> AppSettings => Set<AppSettings>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.UpdatedAtUtc = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
