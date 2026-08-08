using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Domain.Enum;
using AssignmentSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Infrastructure.Seed
{
    public static class DbInitializer
    {
        public const string AdminEmail = "admin@demo.com";
        public const string AdminPassword = "Admin@123";
        public const string TeacherEmail = "teacher@demo.com";
        public const string TeacherPassword = "Teacher@123";
        public const string StudentEmail = "student@demo.com";
        public const string StudentPassword = "Student@123";

        public static async Task SeedAsync(AppDbContext db, IPasswordHasherService passwordHasher, ILogger logger)
        {
            if (await db.Users.AnyAsync())
            {
                logger.LogInformation("Database already seeded — skipping.");
                return;
            }

            logger.LogInformation("Seeding demo users...");

            var admin = new User
            {
                FullName = "System Admin",
                Email = AdminEmail,
                Role = UserRole.Admin,
                IsActive = true
            };
            admin.PasswordHash = passwordHasher.HashPassword(admin, AdminPassword);

            var teacher = new User
            {
                FullName = "Jane Teacher",
                Email = TeacherEmail,
                Role = UserRole.Teacher,
                IsActive = true
            };
            teacher.PasswordHash = passwordHasher.HashPassword(teacher, TeacherPassword);

            var student = new User
            {
                FullName = "Sam Student",
                Email = StudentEmail,
                Role = UserRole.Student,
                IsActive = true
            };
            student.PasswordHash = passwordHasher.HashPassword(student, StudentPassword);

            db.Users.AddRange(admin, teacher, student);
            await db.SaveChangesAsync();

            logger.LogInformation(
                "Seed complete. Demo logins — Admin: {AdminEmail}, Teacher: {TeacherEmail}, Student: {StudentEmail}",
                AdminEmail, TeacherEmail, StudentEmail);
        }


    }
}
