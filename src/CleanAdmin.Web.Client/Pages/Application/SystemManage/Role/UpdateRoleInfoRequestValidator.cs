using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;
using Microsoft.Extensions.Localization;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.Role;

public class UpdateRoleInfoRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsRolesUpdateRoleInfoRequest>
{
    public UpdateRoleInfoRequestValidator(IStringLocalizer<UpdateRoleInfoRequestValidator> l)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(_ => $"{l["Please enter role name"]}");

        RuleFor(x => x.IsDisabled)
            .NotNull().WithMessage(_ => $"{l["Please select status"]}");
    }
}
