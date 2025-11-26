using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calls.Domain.Rooms;

namespace Calls.Infrastructure.Rooms.Repositories;

public class InMemoryRoomRepository : IRoomRepository
{
    private readonly ConcurrentDictionary<Guid, Room> _rooms = new();

    public Task<Room> AddAsync(Room room)
    {
        _rooms[room.RoomId] = room;
        return Task.FromResult(room);
    }

    public Task<bool> DeleteAsync(Guid roomId)
    {
        return Task.FromResult(_rooms.TryRemove(roomId, out _));
    }

    public Task<Room?> GetByIdAsync(Guid roomId)
    {
        _rooms.TryGetValue(roomId, out var room);
        return Task.FromResult(room);
    }

    public Task<List<Room>> GetByUserIdAsync(Guid userId)
    {
        var rooms = _rooms.Values
            .Where(room => room.Participants.Any(participant => participant.UserId == userId))
            .ToList();

        return Task.FromResult(rooms);
    }

    public Task<Room> UpdateAsync(Room room)
    {
        _rooms[room.RoomId] = room;
        return Task.FromResult(room);
    }
}

