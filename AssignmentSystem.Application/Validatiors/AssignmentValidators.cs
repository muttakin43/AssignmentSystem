using AssignmentSystem.Application.DTOs.Assignments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Validatiors
{
    public class CreateAssignmentRequestValidator : AbstractValidator<CreateAssignmentRequest>
    {
        public CreateAssignmentRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.ClassId).NotEmpty();
            RuleFor(x => x.SubjectId).NotEmpty();
            RuleFor(x => x.Deadline).GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");
            RuleFor(x => x.MaxMarks).GreaterThan(0);
        }
    }

    public class UpdateAssignmentRequestValidator : AbstractValidator<UpdateAssignmentRequest>
    {
        public UpdateAssignmentRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.Deadline).GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");
            RuleFor(x => x.MaxMarks).GreaterThan(0);
        }
    }
}
