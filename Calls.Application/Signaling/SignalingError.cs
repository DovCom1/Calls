namespace Calls.Application.Signaling;

public class SignalingError
{
    public SignalingMessageType OriginalType { get; set; }

    public string Message { get; set; } = string.Empty;
}

