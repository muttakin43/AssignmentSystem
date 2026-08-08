using AssignmentSystem.Application.DTOs.Auth;
using AssignmentSystem.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Services
{
    public class AuthService(
        IAppDbContext dbContext,
        IPasswordHasherService passwordHasherService,
        IJwtTokenGenerator jwtTokenGenerator) : IAuthService
    {
        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
        {
            var user = await dbContext.Users
             .FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user is null)
                return null;

            var isPasswordValid = passwordHasherService.VerifyPassword(user, loginRequest.Password);
            if (!isPasswordValid)
                return null;

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
