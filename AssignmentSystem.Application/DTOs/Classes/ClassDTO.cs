using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs.Classes
{
    public record ClassDto(Guid Id, string Name, string? Description, bool IsActive, int StudentCount);

    public record ClassDetailDto(
        Guid Id,
        string Name,
        string? Description,
        bool IsActive,
        int StudentCount,
        IReadOnlyList<ClassSubjectDto> Subjects);

    public record ClassSubjectDto(Guid SubjectId, string SubjectName, string SubjectCode);

    public record CreateClassRequest(string Name, string? Description);

    public record UpdateClassRequest(string Name, string? Description, bool IsActive);
}
