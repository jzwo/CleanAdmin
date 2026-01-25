using Microsoft.Kiota.Abstractions;

namespace NcpAdminAntBlazor.Client.Infrastructure.Http;

/// <summary>
/// this class is used to mark requests that do not require authentication
/// </summary>
public sealed class AnonymousRequestOption : IRequestOption
{
    private AnonymousRequestOption()
    {
    }

    public static readonly AnonymousRequestOption Instance = new();
}