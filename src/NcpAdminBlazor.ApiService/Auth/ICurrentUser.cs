using NcpAdminBlazor.Domain.AggregatesModel.UserAggregate;

namespace NcpAdminBlazor.ApiService.Auth;

public interface ICurrentUser
{
    UserId? UserId { get; }
    string UserName { get; }
    IReadOnlyList<string> PermissionCodes { get; }
}

public class CurrentUser : ICurrentUser
{
    public UserId? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public IReadOnlyList<string> PermissionCodes { get; set; } = [];
}