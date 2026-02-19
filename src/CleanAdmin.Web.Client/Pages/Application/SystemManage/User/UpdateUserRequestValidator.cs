using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;
using Microsoft.Extensions.Localization;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.User;

public class UpdateUserRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsUsersUpdateUserRequest>
{
    public UpdateUserRequestValidator(IStringLocalizer<UpdateUserRequestValidator> l)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(_ => $"{l["Please enter username"]}");

        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage(_ => $"{l["Please enter real name"]}");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage(_ => $"{l["Please enter a valid email address"]}")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
