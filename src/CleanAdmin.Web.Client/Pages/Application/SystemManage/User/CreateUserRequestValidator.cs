using FluentValidation;
using CleanAdmin.Web.Client.ApiSdk.Models;

namespace CleanAdmin.Web.Client.Pages.Application.SystemManage.User;

public class CreateUserRequestValidator : AbstractValidator<CleanAdminApiServiceEndpointsUsersCreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("请输入用户名");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("请输入密码")
            .MinimumLength(6).WithMessage("密码长度不能少于6位");

        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage("请输入真实姓名");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("请输入有效的邮箱地址")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
