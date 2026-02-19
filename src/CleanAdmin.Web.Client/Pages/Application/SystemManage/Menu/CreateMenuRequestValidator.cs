using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;
using Microsoft.Extensions.Localization;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.Menu;

public class CreateMenuRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsMenusCreateMenuRequest>
{
    public CreateMenuRequestValidator(IStringLocalizer<CreateMenuRequestValidator> l)
    {
        RuleFor(x => x.MenuName)
            .NotEmpty().WithMessage(_ => $"{l["Please enter menu name"]}");

        RuleFor(x => x.MenuType)
            .NotNull().WithMessage(_ => $"{l["Please select menu type"]}");

        RuleFor(x => x.RoutePath)
            .NotEmpty().WithMessage(_ => $"{l["Please enter route path"]}");

        RuleFor(x => x.SortOrder)
            .NotNull().WithMessage(_ => $"{l["Please enter sort order"]}");
    }
}
