using AssignmentSystem.Application.DTOs.Auth;
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
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            var db = TestDbContextFactory.Create();
        var hasher = new FakePasswordHasher();
        var user = new User
        {
            FullName = "Test Admin",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            IsActive = true
        };
        user.PasswordHash = hasher.HashPassword(user, "Password123");
        db.Users.Add(user);
        await db.SaveChangesAsync();

            var service = new AuthService(db, hasher, new FakeJwtTokenGenerator(), new FakeLogger<AuthService>());

            // Act
            var result = await service.LoginAsync(new LoginRequestDTO { Email = "admin@test.com", Password = "Password123" });

        // Assert
        result.Should().NotBeNull();
        result!.Role.Should().Be("Admin");
    }

    [Fact]
        public async Task LoginAsync_WrongPassword_ReturnsNull()
        {
            var db = TestDbContextFactory.Create();
            var hasher = new FakePasswordHasher();
            var user = new User { FullName = "Test", Email = "user@test.com", Role = UserRole.Teacher, IsActive = true };
            user.PasswordHash = hasher.HashPassword(user, "CorrectPassword");
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var service = new AuthService(db, hasher, new FakeJwtTokenGenerator(), new FakeLogger<AuthService>());

            var result = await service.LoginAsync(new LoginRequestDTO { Email = "user@test.com", Password = "WrongPassword" });

            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_DeactivatedUser_ReturnsNull()
        {
            var db = TestDbContextFactory.Create();
            var hasher = new FakePasswordHasher();
            var user = new User { FullName = "Inactive", Email = "inactive@test.com", Role = UserRole.Student, IsActive = false };
            user.PasswordHash = hasher.HashPassword(user, "Password123");
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var service = new AuthService(db, hasher, new FakeJwtTokenGenerator(), new FakeLogger<AuthService>());

            var result = await service.LoginAsync(new LoginRequestDTO { Email = "inactive@test.com", Password = "Password123" });

            result.Should().BeNull();
        }
    }
}
