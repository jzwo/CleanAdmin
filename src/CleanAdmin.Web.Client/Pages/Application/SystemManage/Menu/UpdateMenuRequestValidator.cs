using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.Menu;

public class UpdateMenuRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsMenusUpdateMenuRequest>
{
    public UpdateMenuRequestValidator()
    {
        RuleFor(x => x.MenuName)
            .NotEmpty().WithMessage("请输入菜单名称");

        RuleFor(x => x.RoutePath)
            .NotEmpty().WithMessage("请输入路由路径");
    }
}
