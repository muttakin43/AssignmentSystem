using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.UnitTests.TestHelpers
{
    public class FakePasswordHasher : IPasswordHasherService
    {
        public string HashPassword(User user, string plainPassword) => $"HASHED:{plainPassword}";

        public bool VerifyPassword(User user, string plainPassword)
            => user.PasswordHash == $"HASHED:{plainPassword}";
    }
}
