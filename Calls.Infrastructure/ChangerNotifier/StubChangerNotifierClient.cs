using Calls.Application.Interfaces.External;
using Calls.Application.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Calls.Infrastructure.ChangerNotifier;

public class StubChangerNotifierClient : IChangerNotifierClient
{
    private readonly ILogger<StubChangerNotifierClient> _logger;
    private readonly ChangerNotifierOptions _options;

    public StubChangerNotifierClient(
        IOptions<ChangerNotifierOptions> options,
        ILogger<StubChangerNotifierClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task NotifyAsync(ChangerNotifierEnvelope envelope, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Sending message to ChangerNotifier at {BaseAddress} for recipient {Recipient}. Payload type: {Type}",
            _options.BaseAddress,
            envelope.RecipientId,
            envelope.Message.Type);

        return Task.CompletedTask;
    }
}

