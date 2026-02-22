using FastEndpoints;
using CleanAdmin.ApiService.Application.Commands.Users;
using CleanAdmin.ApiService.Auth;

namespace CleanAdmin.ApiService.Endpoints.Users;

public sealed class UpdateCurrentUserProfileEndpoint(IMediator mediator, ICurrentUser currentUser)
    : Endpoint<UpdateCurrentUserProfileRequest, ResponseData>
{
    public override void Configure()
    {
        Post("/api/user/profile/update-basic");
        Description(x => x.WithTags("User"));
    }

    public override async Task HandleAsync(UpdateCurrentUserProfileRequest req, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new KnownException("not logged in");

        await mediator.Send(
            new UpdateCurrentUserBasicInfoCommand(userId, req.RealName, req.Email, req.Phone),
            ct);

        await Send.OkAsync(true.AsResponseData(), ct);
    }
}

public sealed class UpdateCurrentUserProfileRequest
{
    public string RealName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public sealed class UpdateCurrentUserProfileRequestValidator : AbstractValidator<UpdateCurrentUserProfileRequest>
{
    public UpdateCurrentUserProfileRequestValidator()
    {
        RuleFor(x => x.RealName)
            .NotEmpty().WithMessage("姓名不能为空")
            .MaximumLength(50).WithMessage("姓名不能超过50个字符");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(100).WithMessage("邮箱不能超过100个字符");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("手机号不能为空")
            .MaximumLength(20).WithMessage("手机号不能超过20个字符");
    }
}

public sealed class UpdateCurrentUserProfileSummary : Summary<UpdateCurrentUserProfileEndpoint, UpdateCurrentUserProfileRequest>
{
    public UpdateCurrentUserProfileSummary()
    {
        Summary = "更新当前用户基础信息";
        Description = "仅允许更新真实姓名、邮箱、手机号";
    }
}
