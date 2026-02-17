using CleanAdmin.Domain.AggregatesModel.UserAggregate;
using CleanAdmin.Infrastructure.Repositories;
using CleanAdmin.Infrastructure.Utils;

namespace CleanAdmin.ApiService.Application.Commands.Users;

public record LoginUserCommand(UserId UserId, string Password) : ICommand;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("用户ID不能为空");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空");
    }
}

public class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : ICommandHandler<LoginUserCommand>
{
    public async Task Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(request.UserId, cancellationToken) ??
                   throw new KnownException("用户不存在");

        var verified = passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password);
        if (!verified)
        {
            throw new KnownException("用户名或密码错误");
        }

        user.Login();
    }
}