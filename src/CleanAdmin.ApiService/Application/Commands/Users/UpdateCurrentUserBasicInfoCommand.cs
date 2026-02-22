using CleanAdmin.Domain.AggregatesModel.UserAggregate;
using CleanAdmin.Infrastructure.Repositories;

namespace CleanAdmin.ApiService.Application.Commands.Users;

public sealed record UpdateCurrentUserBasicInfoCommand(
    UserId UserId,
    string RealName,
    string Email,
    string Phone) : ICommand;

public sealed class UpdateCurrentUserBasicInfoCommandValidator : AbstractValidator<UpdateCurrentUserBasicInfoCommand>
{
    public UpdateCurrentUserBasicInfoCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");

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

public sealed class UpdateCurrentUserBasicInfoCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateCurrentUserBasicInfoCommand>
{
    public async Task Handle(UpdateCurrentUserBasicInfoCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken)
                   ?? throw new KnownException($"未找到用户，UserId = {request.UserId}");

        user.UpdateBasicInfo(request.RealName, request.Email, request.Phone);
    }
}
