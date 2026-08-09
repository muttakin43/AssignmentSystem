using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Exceptions
{
    public class NotFoundException(string message) : Exception(message);
    public class ConflictException(string message) : Exception(message);
    public class ValidationException(string message) : Exception(message);
    public class BusinessRuleException(string message) : Exception(message);
    public class ForbiddenException(string message) : Exception(message);

}
