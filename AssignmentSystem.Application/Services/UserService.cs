using AssignmentSystem.Application.Common;
using AssignmentSystem.Application.DTOs.Users;
using AssignmentSystem.Application.Exceptions;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Services
{
    public class UserService(
        IAppDbContext dbContext,
        IPasswordHasherService passwordHasher) : IUserService
    {
        public async Task<PageResult<UserDto>> GetPagedAsync(UserQuery query, CancellationToken ct = default)
        {
            var users = dbContext.Users.Include(u => u.Class).AsQueryable();

            if (query.Role is not null)
            {
                users = users.Where(u => u.Role == query.Role);
            }

            if (query.ClassId is not null)
            {
                users = users.Where(u => u.ClassId == query.ClassId);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                users = users.Where(u => u.FullName.ToLower().Contains(search) || u.Email.ToLower().Contains(search));
            }

            var paged = await users.OrderBy(u => u.FullName).Select(ToDtoExpression).ToPagedResultAsync(query, ct);
            return paged;
        }
        public async Task ChangePasswordAsync(Guid id, ChangePasswordRequest request, Guid requestingUserId, UserRole requestingUserRole, CancellationToken ct = default)
        {
            if (requestingUserRole != UserRole.Admin && requestingUserId != id)
            {
                throw new ForbiddenException("You may only change your own password.");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
                ?? throw new NotFoundException($"User '{id}' was not found.");

            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            await dbContext.SaveChangesAsync(ct);
        }

        public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
        {
            var emailTaken = await dbContext.Users.AnyAsync(u => u.Email == request.Email, ct);
            if (emailTaken)
            {
                throw new ConflictException($"A user with email '{request.Email}' already exists.");
            }

            if (request.Role == UserRole.Student && request.ClassId is null)
            {
                throw new BusinessRuleException("A class must be assigned when creating a student.");
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Role = request.Role,
                ClassId = request.Role == UserRole.Student ? request.ClassId : null,
                IsActive = true
            };
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(ct);

            return await GetByIdAsync(user.Id, ct);
        }

        public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var user = await dbContext.Users.Include(u => u.Class)
             .Where(u => u.Id == id)
             .Select(ToDtoExpression)
             .FirstOrDefaultAsync(ct)
             ?? throw new NotFoundException($"User '{id}' was not found.");

            return user;
        }

        

        public async Task<UserDto> SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
              ?? throw new NotFoundException($"User '{id}' was not found.");

            user.IsActive = isActive;
            await dbContext.SaveChangesAsync(ct);
            return await GetByIdAsync(id, ct);
        }

        public async  Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new NotFoundException($"User '{id}' was not found.");

            var emailTaken = await dbContext.Users.AnyAsync(u => u.Email == request.Email && u.Id != id, ct);
            if (emailTaken)
            {
                throw new ConflictException($"A user with email '{request.Email}' already exists.");
            }

            if (request.Role == UserRole.Student && request.ClassId is null)
            {
                throw new BusinessRuleException("A class must be assigned when the role is Student.");
            }

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.Role = request.Role;
            user.ClassId = request.Role == UserRole.Student ? request.ClassId : null;

            await dbContext.SaveChangesAsync(ct);
            return await GetByIdAsync(id, ct);
        }
        private static readonly System.Linq.Expressions.Expression<Func<User, UserDto>> ToDtoExpression =
        u => new UserDto(
            u.Id, u.FullName, u.Email, u.Role, u.ClassId,
            u.Class == null ? null : u.Class.Name, u.IsActive, u.CreatedAtUtc);
    }
}
