using AssignmentSystem.Application.DTOs.Subjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Validatiors
{
    public class CreateSubjectRequestValidator : AbstractValidator<CreateSubjectRequest>
    {
        public CreateSubjectRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Code).MaximumLength(20);
        }
    }

    public class UpdateSubjectRequestValidator : AbstractValidator<UpdateSubjectRequest>
    {
        public UpdateSubjectRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Code).MaximumLength(20);
        }
    }
}
