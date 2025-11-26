using System;
using Calls.Application.Requests;

namespace Calls.Application.Notifications;

public class ChangerNotifierEnvelope
{
    public Guid RecipientId { get; set; }

    public SignalingMessageRequest Message { get; set; } = new();
}

