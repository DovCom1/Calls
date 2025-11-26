using System;

namespace Calls.Domain.Rooms;

public class RoomParticipant
{
    public RoomParticipant(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));

        UserId = userId;
        Settings = new ParticipantSettings();
        SmallUserInfo = new SmallUserInfo();
    }

    public Guid UserId { get; }

    public ParticipantSettings Settings { get; }

    public SmallUserInfo SmallUserInfo { get; }
}

