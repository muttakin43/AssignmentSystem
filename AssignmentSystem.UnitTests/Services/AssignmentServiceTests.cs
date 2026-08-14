using AssignmentSystem.Application.DTOs.Assignments;
using AssignmentSystem.Application.Exceptions;
using AssignmentSystem.Application.Services;
using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Domain.Enum;

using AssignmentSystem.UnitTests.TestHelpers;
using FluentAssertions;
using Xunit;

namespace AssignmentSystem.Tests.Services;

public class AssignmentServiceTests
{
    private static async Task<(AssignmentSystem.Infrastructure.Persistence.AppDbContext db, User teacher, ClassCourse cls, Subject subject)> SeedBasicDataAsync()
    {
        var db = TestDbContextFactory.Create();

        var teacher = new User { FullName = "Karim", Email = "karim@test.com", Role = UserRole.Teacher, IsActive = true, PasswordHash = "x" };
        var cls = new ClassCourse { Name = "Class 9" };
        var subject = new Subject { Name = "Math", Code = "MATH101" };

        db.Users.Add(teacher);
        db.Classes.Add(cls);
        db.Subjects.Add(subject);
        await db.SaveChangesAsync();

        return (db, teacher, cls, subject);
    }

    [Fact]
    public async Task CreateAsync_TeacherNotAssigned_ThrowsForbidden()
    {
        var (db, teacher, cls, subject) = await SeedBasicDataAsync();
        var service = new AssignmentService(db);

        var request = new CreateAssignmentRequest("Homework", "desc", cls.Id, subject.Id, DateTime.UtcNow.AddDays(5), 100, true);

        Func<Task> act = async () => await service.CreateAsync(request, teacher.Id);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task CreateAsync_TeacherAssigned_CreatesAssignmentAsDraft()
    {
        var (db, teacher, cls, subject) = await SeedBasicDataAsync();
        db.TeacherAssignments.Add(new TeacherAssignment { TeacherId = teacher.Id, ClassId = cls.Id, SubjectId = subject.Id, IsActive = true });
        await db.SaveChangesAsync();

        var service = new AssignmentService(db);
        var request = new CreateAssignmentRequest("Homework", "desc", cls.Id, subject.Id, DateTime.UtcNow.AddDays(5), 100, true);

        var result = await service.CreateAsync(request, teacher.Id);

        result.Should().NotBeNull();
        result.Status.Should().Be(AssignmentStatus.Draft);
    }

    [Fact]
    public async Task CreateAsync_PastDeadline_ThrowsValidationException()
    {
        var (db, teacher, cls, subject) = await SeedBasicDataAsync();
        db.TeacherAssignments.Add(new TeacherAssignment { TeacherId = teacher.Id, ClassId = cls.Id, SubjectId = subject.Id, IsActive = true });
        await db.SaveChangesAsync();

        var service = new AssignmentService(db);
        var request = new CreateAssignmentRequest("Homework", "desc", cls.Id, subject.Id, DateTime.UtcNow.AddDays(-1), 100, true);

        Func<Task> act = async () => await service.CreateAsync(request, teacher.Id);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateAsync_DifferentTeacher_ThrowsForbidden()
    {
        var (db, teacher, cls, subject) = await SeedBasicDataAsync();
        db.TeacherAssignments.Add(new TeacherAssignment { TeacherId = teacher.Id, ClassId = cls.Id, SubjectId = subject.Id, IsActive = true });
        await db.SaveChangesAsync();

        var service = new AssignmentService(db);
        var createRequest = new CreateAssignmentRequest("Homework", "desc", cls.Id, subject.Id, DateTime.UtcNow.AddDays(5), 100, true);
        var created = await service.CreateAsync(createRequest, teacher.Id);

        var otherTeacherId = Guid.NewGuid();
        var updateRequest = new UpdateAssignmentRequest("Hacked", "desc", DateTime.UtcNow.AddDays(10), 50, true);

        Func<Task> act = async () => await service.UpdateAsync(created.Id, updateRequest, otherTeacherId, UserRole.Teacher);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task PublishAsync_AlreadyPublished_ThrowsConflict()
    {
        var (db, teacher, cls, subject) = await SeedBasicDataAsync();
        db.TeacherAssignments.Add(new TeacherAssignment { TeacherId = teacher.Id, ClassId = cls.Id, SubjectId = subject.Id, IsActive = true });
        await db.SaveChangesAsync();

        var service = new AssignmentService(db);
        var created = await service.CreateAsync(
            new CreateAssignmentRequest("Homework", "desc", cls.Id, subject.Id, DateTime.UtcNow.AddDays(5), 100, true), teacher.Id);

        await service.PublishAsync(created.Id, teacher.Id, UserRole.Teacher);

        Func<Task> act = async () => await service.PublishAsync(created.Id, teacher.Id, UserRole.Teacher);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task DeleteAsync_WithSubmissions_ThrowsConflict()
    {
        var (db, teacher, cls, subject) = await SeedBasicDataAsync();
        db.TeacherAssignments.Add(new TeacherAssignment { TeacherId = teacher.Id, ClassId = cls.Id, SubjectId = subject.Id, IsActive = true });
        await db.SaveChangesAsync();

        var service = new AssignmentService(db);
        var created = await service.CreateAsync(
            new CreateAssignmentRequest("Homework", "desc", cls.Id, subject.Id, DateTime.UtcNow.AddDays(5), 100, true), teacher.Id);
        await service.PublishAsync(created.Id, teacher.Id, UserRole.Teacher);

        var student = new User { FullName = "Stud", Email = "stud@test.com", Role = UserRole.Student, IsActive = true, PasswordHash = "x", ClassId = cls.Id };
        db.Users.Add(student);
        db.Submissions.Add(new Submission
        {
            AssignmentId = created.Id,
            StudentId = student.Id,
            TextAnswer = "answer",
            Status = SubmissionStatus.Submitted,
            SubmittedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        Func<Task> act = async () => await service.DeleteAsync(created.Id, teacher.Id, UserRole.Teacher);

        await act.Should().ThrowAsync<ConflictException>();
    }
}