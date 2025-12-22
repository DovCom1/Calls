
using Calls.Application.DTO;
using Calls.Application.Notifications;

namespace Calls.Application.Extensions;

public static class ChangerNotifierEnvelopeExtensions
{
    public static CallEventDto ToCallEventDto(this Notifications.ChangerNotifierEnvelope envelope)
    {
        if (envelope == null)
            throw new ArgumentNullException(nameof(envelope));

        return new CallEventDto
        {
            TypeDto = envelope.Message.Type.ToString(),
            SenderId = envelope.Message.From,
            ReceiverId = envelope.RecipientId,
            Payload = envelope.Message.Payload
        };
    }
}