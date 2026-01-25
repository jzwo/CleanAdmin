using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace NcpAdminAntBlazor.Client.Infrastructure.Http;

public sealed class BearerTokenAuthenticationProvider(IAuthenticationProvider authenticationProvider)
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

        await authenticationProvider.AuthenticateRequestAsync(request, additionalAuthenticationContext,
            cancellationToken);
    }
}