using AssignmentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces;
public record GeneratedToken(string Token, DateTime ExpiresAtUtc);
public interface IJwtTokenGenerator
{
    GeneratedToken GenerateToken(User user);
}
