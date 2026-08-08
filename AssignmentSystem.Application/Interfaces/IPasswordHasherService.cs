using AssignmentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string plainpassword);
        bool VerifyPassword(User user, string plainpassword);
    }
}
