using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;
using Microsoft.Extensions.Localization;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.Menu;

public class UpdateMenuRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsMenusUpdateMenuRequest>
{
    public UpdateMenuRequestValidator(IStringLocalizer<UpdateMenuRequestValidator> l)
    {
        RuleFor(x => x.MenuName)
            .NotEmpty().WithMessage(_ => $"{l["Please enter menu name"]}");

        RuleFor(x => x.RoutePath)
            .NotEmpty().WithMessage(_ => $"{l["Please enter route path"]}");
    }
}
