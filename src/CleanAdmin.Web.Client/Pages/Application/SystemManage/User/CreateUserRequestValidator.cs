using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;
using Microsoft.Extensions.Localization;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.User;

public class CreateUserRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsUsersCreateUserRequest>
{
    public CreateUserRequestValidator(IStringLocalizer<CreateUserRequestValidator> l)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(_ => $"{l["Please enter username"]}");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_ => $"{l["Please enter password"]}")
            .MinimumLength(6).WithMessage(_ => $"{l["Password must be at least 6 characters"]}");

        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage(_ => $"{l["Please enter real name"]}");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage(_ => $"{l["Please enter a valid email address"]}")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
