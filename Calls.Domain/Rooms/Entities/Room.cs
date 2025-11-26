using System;
using System.Collections.Generic;
using System.Linq;

namespace Calls.Domain.Rooms;

public class Room
{
    private readonly List<RoomParticipant> _participants = new();

    private Room(Guid roomId, string name)
    {
        RoomId = roomId;
        Name = name;
    }

    public Guid RoomId { get; }

    public string Name { get; private set; }

    public IReadOnlyCollection<RoomParticipant> Participants => _participants;

    public static Room Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Room name is required.", nameof(name));

        return new Room(Guid.NewGuid(), name.Trim());
    }

    public bool AddParticipant(Guid userId)
    {
        if (_participants.Any(p => p.UserId == userId))
            return false;

        _participants.Add(new RoomParticipant(userId));
        return true;
    }

    public bool RemoveParticipant(Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId);
        if (participant == null)
            return false;

        _participants.Remove(participant);
        return true;
    }
}

