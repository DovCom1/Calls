using System;
using System.Collections.Generic;

namespace Calls.Application.Signaling;

public class SignalingRoomInfo
{
    public Guid RoomId { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Guid> Participants { get; set; } = new();
}

