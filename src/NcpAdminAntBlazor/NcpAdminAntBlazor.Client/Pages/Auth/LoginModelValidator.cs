using FluentValidation;

namespace NcpAdminAntBlazor.Client.Pages.Auth;

public class LoginModelValidator : AbstractValidator<LoginModel>
{
    public LoginModelValidator()
    {
        RuleFor(p => p.Username)
            .NotEmpty().WithMessage("Username name is required")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}