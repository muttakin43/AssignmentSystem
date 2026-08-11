using AssignmentSystem.Application.DTOs.Submission;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Validatiors
{
    public class GradeSubmissionRequestValidator : AbstractValidator<GradeSubmissionRequest>
    {
        public GradeSubmissionRequestValidator()
        {
            RuleFor(x => x.MarksObtained).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Feedback).MaximumLength(1000);
        }
    }

    public class ChangeSubmissionStatusRequestValidator : AbstractValidator<ChangeSubmissionStatusRequest>
    {
        public ChangeSubmissionStatusRequestValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
