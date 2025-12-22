using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Calls.Application.Extensions;
using Calls.Application.Interfaces.External;
using Calls.Application.Notifications;
using Microsoft.Extensions.Logging;

namespace Calls.Infrastructure.ChangerNotifier;

public class HttpChangerNotifierClient(
    HttpClient httpClient,
    ILogger<HttpChangerNotifierClient> logger) : IChangerNotifierClient
{
    const string requestUri = "/api/signaling";

    public async Task NotifyAsync(ChangerNotifierEnvelope envelope, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Sending signaling message to CN {BaseAddress}{Path} for recipient {RecipientId}",
            httpClient.BaseAddress, requestUri, envelope.RecipientId);

        using var response = await httpClient.PostAsJsonAsync(requestUri, envelope.ToCallEventDto(), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "ChangerNotifier returned non-success status code {StatusCode}. Content: {Content}",
                (int)response.StatusCode,
                content);
        }
    }
}

