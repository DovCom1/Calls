using System;
using Calls.Application.Signaling;

namespace Calls.Application.Requests;

public class SignalingMessageRequest
{
    public SignalingMessageType Type { get; set; }

    public Guid From { get; set; }

    public Guid? To { get; set; }

    public SignalingPayload Payload { get; set; } = new();
}

