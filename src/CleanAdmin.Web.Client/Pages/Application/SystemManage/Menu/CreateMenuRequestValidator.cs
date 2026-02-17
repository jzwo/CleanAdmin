using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.Menu;

public class CreateMenuRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsMenusCreateMenuRequest>
{
    public CreateMenuRequestValidator()
    {
        RuleFor(x => x.MenuName)
            .NotEmpty().WithMessage("请输入菜单名称");

        RuleFor(x => x.MenuType)
            .NotNull().WithMessage("请选择菜单类型");

        RuleFor(x => x.RoutePath)
            .NotEmpty().WithMessage("请输入路由路径");

        RuleFor(x => x.SortOrder)
            .NotNull().WithMessage("请输入排序号");
    }
}
