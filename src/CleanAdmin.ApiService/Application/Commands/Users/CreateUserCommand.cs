using CleanAdmin.ApiService.Application.Queries.Roles;
using CleanAdmin.ApiService.Application.Queries.Users;
using CleanAdmin.Domain.AggregatesModel.UserAggregate;
using CleanAdmin.Infrastructure.Repositories;
using CleanAdmin.Infrastructure.Utils;

namespace CleanAdmin.ApiService.Application.Commands.Users;

public record CreateUserCommand(
    string Username,
    string Password,
    string RealName,
    string Email,
    string Phone,
    List<RoleDetailDto> RoleDetails) : ICommand<UserId>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IMediator mediator)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名不能超过50个字符")
            .MustAsync(async (username, cancellationToken) =>
                !await mediator.Send(new CheckUserExistsByUsernameQuery(username), cancellationToken))
            .WithMessage("用户名已存在");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MaximumLength(50).WithMessage("密码长度不能超过50位");

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

        RuleFor(x => x.RoleDetails)
            .NotNull().WithMessage("角色详情不能为空");
    }
}

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, UserId>
{
    public async Task<UserId> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = passwordHasher.HashPassword(request.Password);

        // 创建UserRole集合
        var userRoles = request.RoleDetails
            .Select(r => new UserRole(r.RoleId, r.RoleName))
            .ToList();

        var rolePermissionMappings = request.RoleDetails
            .Select(r => (r.RoleId, PermissionCodes: r.PermissionCodes.AsEnumerable()))
            .ToList();

        var user = new User(
            username: request.Username,
            passwordHash: hashedPassword,
            realName: request.RealName,
            email: request.Email,
            phone: request.Phone,
            userRoles: userRoles,
            rolePermissionMappings: rolePermissionMappings
        );

        await userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}
