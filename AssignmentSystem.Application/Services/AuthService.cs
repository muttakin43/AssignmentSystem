using AssignmentSystem.Application.DTOs.Auth;
using AssignmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Services
{
    public class AuthService(
        IAppDbContext dbContext,
        IPasswordHasherService passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger) : IAuthService
    {
        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
            {
                logger.LogWarning("Login failed: no user with email {Email}", request.Email);
                return null;
            }

            if (!user.IsActive)
            {
                logger.LogWarning("Login failed: user {Email} is deactivated", request.Email);
                return null;
            }

            var isPasswordValid = passwordHasher.VerifyPassword(user, request.Password);
            if (!isPasswordValid)
            {
                logger.LogWarning("Login failed: invalid password for {Email}", request.Email);
                return null;
            }

            logger.LogInformation("User {Email} logged in successfully with role {Role}", user.Email, user.Role);

            var generatedToken = jwtTokenGenerator.GenerateToken(user);

            return new LoginResponseDTO
            {
                Token = generatedToken.Token,
                ExpiresAtUtc = generatedToken.ExpiresAtUtc,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
