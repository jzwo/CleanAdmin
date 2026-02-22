using FastEndpoints;
using CleanAdmin.ApiService.Application.Commands.Users;
using CleanAdmin.ApiService.Auth;

namespace CleanAdmin.ApiService.Endpoints.Users;

public sealed class ChangeCurrentUserPasswordEndpoint(IMediator mediator, ICurrentUser currentUser)
    : Endpoint<ChangeCurrentUserPasswordRequest, ResponseData>
{
    public override void Configure()
    {
        Post("/api/user/profile/change-password");
        Description(x => x.WithTags("User"));
    }

    public override async Task HandleAsync(ChangeCurrentUserPasswordRequest req, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new KnownException("not logged in");

        await mediator.Send(new ChangePasswordCommand(userId, req.OldPassword, req.NewPassword), ct);
        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class ChangeCurrentUserPasswordRequest
{
    public string OldPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}

public sealed class ChangeCurrentUserPasswordRequestValidator : AbstractValidator<ChangeCurrentUserPasswordRequest>
{
    public ChangeCurrentUserPasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("旧密码不能为空");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("新密码不能为空")
            .MinimumLength(6).WithMessage("新密码长度不能少于6位")
            .MaximumLength(50).WithMessage("新密码长度不能超过50位");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("确认密码不能为空")
            .Equal(x => x.NewPassword).WithMessage("两次输入的新密码不一致");
    }
}

public sealed class ChangeCurrentUserPasswordSummary : Summary<ChangeCurrentUserPasswordEndpoint, ChangeCurrentUserPasswordRequest>
{
    public ChangeCurrentUserPasswordSummary()
    {
        Summary = "修改当前用户密码";
        Description = "校验旧密码后更新为新密码";
    }
}
