using AssignmentSystem.Application.DTOs.TeacherAssignments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Validatiors
{
    public class CreateTeacherAssignmentRequestValidator : AbstractValidator<CreateTeacherAssignmentRequest>
    {
        public CreateTeacherAssignmentRequestValidator()
        {
            RuleFor(x => x.TeacherId).NotEmpty();
            RuleFor(x => x.ClassId).NotEmpty();
            RuleFor(x => x.SubjectId).NotEmpty();
        }
    }
}
