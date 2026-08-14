using AssignmentSystem.Application.DTOs.TeacherAssignments;
using AssignmentSystem.Application.Exceptions;
using AssignmentSystem.Application.Services;
using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Domain.Enum;
using AssignmentSystem.UnitTests.TestHelpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.UnitTests.Services
{
    public class TeacherAssignmentServiceTests
    {
        [Fact]
        public async Task CreateAsync_DuplicateActiveAssignment_ThrowsConflict()
        {
            var db = TestDbContextFactory.Create();
            var teacher1 = new User { FullName = "T1", Email = "t1@test.com", Role = UserRole.Teacher, IsActive = true, PasswordHash = "x" };
            var teacher2 = new User { FullName = "T2", Email = "t2@test.com", Role = UserRole.Teacher, IsActive = true, PasswordHash = "x" };
            var cls = new ClassCourse { Name = "Class 9" };
            var subject = new Subject { Name = "Math" };
            db.Users.AddRange(teacher1, teacher2);
            db.Classes.Add(cls);
            db.Subjects.Add(subject);
            await db.SaveChangesAsync();

            var service = new TeacherAssignmentService(db);
            await service.CreateAsync(new CreateTeacherAssignmentRequest(teacher1.Id, cls.Id, subject.Id));

            Func<Task> act = async () => await service.CreateAsync(new CreateTeacherAssignmentRequest(teacher2.Id, cls.Id, subject.Id));

            await act.Should().ThrowAsync<ConflictException>();
        }

        [Fact]
        public async Task CreateAsync_NonTeacherUser_ThrowsBusinessRuleException()
        {
            var db = TestDbContextFactory.Create();
            var student = new User { FullName = "S1", Email = "s1@test.com", Role = UserRole.Student, IsActive = true, PasswordHash = "x" };
            var cls = new ClassCourse { Name = "Class 9" };
            var subject = new Subject { Name = "Math" };
            db.Users.Add(student);
            db.Classes.Add(cls);
            db.Subjects.Add(subject);
            await db.SaveChangesAsync();

            var service = new TeacherAssignmentService(db);

            Func<Task> act = async () => await service.CreateAsync(new CreateTeacherAssignmentRequest(student.Id, cls.Id, subject.Id));

            await act.Should().ThrowAsync<BusinessRuleException>();
        }
    }
}
