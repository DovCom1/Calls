using Calls.Application.Notifications;

namespace Calls.Application.Interfaces.External;

public interface IChangerNotifierClient
{
    Task NotifyAsync(ChangerNotifierEnvelope envelope, CancellationToken cancellationToken = default);
}

