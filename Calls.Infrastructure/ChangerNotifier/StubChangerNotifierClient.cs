using Calls.Application.Interfaces.External;
using Calls.Application.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Calls.Infrastructure.ChangerNotifier;

public class StubChangerNotifierClient : IChangerNotifierClient
{
    private readonly ILogger<StubChangerNotifierClient> _logger;

    public StubChangerNotifierClient(
        ILogger<StubChangerNotifierClient> logger)
    {
        _logger = logger;
    }

    public Task NotifyAsync(ChangerNotifierEnvelope envelope, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Sending message to ChangerNotifier at for recipient {Recipient}. Payload type: {Type}",
            envelope.RecipientId,
            envelope.Message.Type);

        return Task.CompletedTask;
    }
}

