using Calls.Application.Signaling;

namespace Calls.Application.DTO;
public class CallEventDto
{
    public string TypeDto { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    public SignalingPayload Payload { get; set; } = new();
}
