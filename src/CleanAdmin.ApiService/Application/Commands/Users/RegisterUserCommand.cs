using CleanAdmin.ApiService.Application.Queries.Users;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;
using CleanAdmin.Infrastructure.Repositories;
using CleanAdmin.Infrastructure.Utils;

namespace CleanAdmin.ApiService.Application.Commands.Users;

public record RegisterUserCommand(
    string Username,
    string Password
) : ICommand<UserId>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(IMediator mediator)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50)
            .WithMessage("用户名不能超过50个字符")
            .MustAsync(async (name, cancellation) =>
                !await mediator.Send(new CheckUserExistsByUsernameQuery(name), cancellation))
            .WithMessage("用户名已存在");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MaximumLength(50).WithMessage("密码长度不能超过50位");
    }
}

public class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterUserCommand, UserId>
{
    public async Task<UserId> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = passwordHasher.HashPassword(request.Password);
        var user = new User(
            username: request.Username,
            passwordHash: hashedPassword,
            realName: string.Empty,
            email: string.Empty,
            phone: string.Empty,
            userRoles: [],
            userPermissions: []
        );

        await userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}
