using AssignmentSystem.Application.DTOs.Submission;
using AssignmentSystem.Application.Exceptions;
using AssignmentSystem.Application.Services;
using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Domain.Enum;
using AssignmentSystem.Infrastructure.Persistence;
using AssignmentSystem.UnitTests.TestHelpers;

using FluentAssertions;
using Xunit;

namespace AssignmentSystem.Tests.Services;

public class SubmissionServiceTests
{
    private static async Task<(AppDbContext db, User student, Assignment assignment)> SeedAsync(bool published = true, DateTime? deadline = null)
    {
        var db = TestDbContextFactory.Create();

        var teacher = new User { FullName = "Teacher", Email = "t@test.com", Role = UserRole.Teacher, IsActive = true, PasswordHash = "x" };
        var cls = new ClassCourse { Name = "Class 9" };
        var subject = new Subject { Name = "Math" };
        db.Users.Add(teacher);
        db.Classes.Add(cls);
        db.Subjects.Add(subject);
        await db.SaveChangesAsync();

        var student = new User { FullName = "Student", Email = "s@test.com", Role = UserRole.Student, IsActive = true, PasswordHash = "x", ClassId = cls.Id };
        db.Users.Add(student);

        var assignment = new Assignment
        {
            Title = "HW1",
            ClassId = cls.Id,
            SubjectId = subject.Id,
            TeacherId = teacher.Id,
            Deadline = deadline ?? DateTime.UtcNow.AddDays(5),
            MaxMarks = 100,
            AllowUpdateAfterSubmit = true,
            Status = published ? AssignmentStatus.Published : AssignmentStatus.Draft
        };
        db.Assignments.Add(assignment);
        await db.SaveChangesAsync();

        return (db, student, assignment);
    }

    [Fact]
    public async Task CreateAsync_ValidSubmission_Succeeds()
    {
        var (db, student, assignment) = await SeedAsync();
        var service = new SubmissionService(db, new FakeDateTimeProvider(), new FakeFileStorageService());

        var result = await service.CreateAsync(assignment.Id, student.Id, "My answer", null);

        result.Should().NotBeNull();
        result.Status.Should().Be(SubmissionStatus.Submitted);
    }

    [Fact]
    public async Task CreateAsync_DuplicateSubmission_ThrowsConflict()
    {
        var (db, student, assignment) = await SeedAsync();
        var service = new SubmissionService(db, new FakeDateTimeProvider(), new FakeFileStorageService());

        await service.CreateAsync(assignment.Id, student.Id, "First answer", null);

        Func<Task> act = async () => await service.CreateAsync(assignment.Id, student.Id, "Second answer", null);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task GradeAsync_MarksExceedMax_ThrowsBusinessRuleException()
    {
        var (db, student, assignment) = await SeedAsync();
        var service = new SubmissionService(db, new FakeDateTimeProvider(), new FakeFileStorageService());
        var submission = await service.CreateAsync(assignment.Id, student.Id, "Answer", null);

        Func<Task> act = async () => await service.GradeAsync(
            submission.Id, new GradeSubmissionRequest(150, "Too high"), assignment.TeacherId, UserRole.Teacher);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }
}