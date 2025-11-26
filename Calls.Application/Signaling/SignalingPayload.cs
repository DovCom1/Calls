namespace Calls.Application.Signaling;

public class SignalingPayload
{
    public SignalingRoomInfo? RoomInfo { get; set; }

    public SignalingError? Error { get; set; }

    public object? Sdp { get; set; }

    public object? IceCandidate { get; set; }

    public int? Emotion { get; set; }
}

