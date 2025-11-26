using Calls.Application.Requests;

namespace Calls.Application.Interfaces;

public interface ISignalingService
{
    Task HandleAsync(SignalingMessageRequest request, CancellationToken cancellationToken = default);
}

