using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Infrastructure.Auth
{
    public class PasswordHasherWrapper : IPasswordHasherService
    {
        private readonly PasswordHasher<User> _hasher = new();
        public string HashPassword(User user, string plainpassword)
        => _hasher.HashPassword(user, plainpassword);

        public bool VerifyPassword(User user, string plainpassword)
        {
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, plainpassword);
            return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
