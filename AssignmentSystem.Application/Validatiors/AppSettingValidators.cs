using AssignmentSystem.Application.DTOs.AppSettings;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Validatiors
{
    public class CreateAppSettingRequestValidator : AbstractValidator<CreateAppSettingRequest>
    {
        public CreateAppSettingRequestValidator()
        {
            RuleFor(x => x.Key).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Value).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public class UpdateAppSettingRequestValidator : AbstractValidator<UpdateAppSettingRequest>
    {
        public UpdateAppSettingRequestValidator()
        {
            RuleFor(x => x.Value).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}
