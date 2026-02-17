using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.User;

public class UpdateUserRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsUsersUpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("请输入用户名");

        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage("请输入真实姓名");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("请输入有效的邮箱地址")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
