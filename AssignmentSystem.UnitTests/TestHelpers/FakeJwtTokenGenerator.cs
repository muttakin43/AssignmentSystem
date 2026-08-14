using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.UnitTests.TestHelpers
{
    public class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
        public GeneratedToken GenerateToken(User user)
       => new("fake-token", DateTime.UtcNow.AddHours(1));
    }
}
