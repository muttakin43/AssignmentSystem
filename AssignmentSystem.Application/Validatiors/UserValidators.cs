using AssignmentSystem.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using AssignmentSystem.Domain.Enum;

namespace AssignmentSystem.Application.Validatiors
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Role).IsInEnum();
            RuleFor(x => x.ClassId)
                .NotNull()
                .WithMessage("A class must be assigned when the role is Student.")
                .When(x => x.Role == UserRole.Student);
        }
    }

    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
            RuleFor(x => x.Role).IsInEnum();
            RuleFor(x => x.ClassId)
                .NotNull()
                .WithMessage("A class must be assigned when the role is Student.")
                .When(x => x.Role == UserRole.Student);
        }
    }

    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).MaximumLength(100);
        }
    }
}
