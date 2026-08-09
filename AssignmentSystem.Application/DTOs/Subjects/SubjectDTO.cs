using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs.Subjects
{
    public record SubjectDto(Guid Id, string Name, string Code, bool IsActive);

    public record CreateSubjectRequest(string Name, string Code);

    public record UpdateSubjectRequest(string Name, string Code,bool IsActive);
}
