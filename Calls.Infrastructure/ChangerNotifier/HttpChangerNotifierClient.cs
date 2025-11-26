using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Calls.Application.Interfaces.External;
using Calls.Application.Notifications;
using Microsoft.Extensions.Logging;

namespace Calls.Infrastructure.ChangerNotifier;

public class HttpChangerNotifierClient : IChangerNotifierClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpChangerNotifierClient> _logger;
    const string requestUri = "/api/signaling";

    public HttpChangerNotifierClient(
        HttpClient httpClient,
        ILogger<HttpChangerNotifierClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task NotifyAsync(ChangerNotifierEnvelope envelope, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Sending signaling message to CN {BaseAddress}{Path} for recipient {RecipientId}",
            _httpClient.BaseAddress, requestUri, envelope.RecipientId);

        using var response = await _httpClient.PostAsJsonAsync(requestUri, envelope, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning(
                "ChangerNotifier returned non-success status code {StatusCode}. Content: {Content}",
                (int)response.StatusCode,
                content);
        }
    }
}

