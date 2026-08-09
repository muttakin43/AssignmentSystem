using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs.TeacherAssignments
{
    public record TeacherAssignmentDto(
      Guid Id,
      Guid TeacherId,
      string TeacherName,
      Guid ClassId,
      string ClassName,
      Guid SubjectId,
      string SubjectName,
      bool IsActive);

    public record CreateTeacherAssignmentRequest(Guid TeacherId, Guid ClassId, Guid SubjectId);
}
