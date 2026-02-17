using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.Role;

public class UpdateRoleInfoRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsRolesUpdateRoleInfoRequest>
{
    public UpdateRoleInfoRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("请输入角色名称");

        RuleFor(x => x.IsDisabled)
            .NotNull().WithMessage("请选择状态");
    }
}
