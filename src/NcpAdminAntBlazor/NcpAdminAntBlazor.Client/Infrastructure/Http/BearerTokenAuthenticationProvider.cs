using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace NcpAdminAntBlazor.Client.Infrastructure.Http;

public sealed class BearerTokenAuthenticationProvider(
    BaseBearerTokenAuthenticationProvider baseBearerTokenAuthenticationProvider)
    : IAuthenticationProvider
{
    public async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        if (request.RequestOptions.Any(x => x is AnonymousRequestOption))
        {
            return;
        }

        await baseBearerTokenAuthenticationProvider.AuthenticateRequestAsync(request, additionalAuthenticationContext,
            cancellationToken);
    }
}