using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Calls.Domain.Rooms;
using Calls.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Calls.Infrastructure.Rooms.Repositories;

public class EfRoomRepository : IRoomRepository
{
    private readonly CallsDbContext _context;

    public EfRoomRepository(CallsDbContext context)
    {
        _context = context;
    }

    public async Task<Room> AddAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<Room?> GetByIdAsync(Guid roomId)
    {
        return await _context.Rooms
            .Include(r => r.Participants)
            .FirstOrDefaultAsync(r => r.RoomId == roomId);
    }

    public async Task<List<Room>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Rooms
            .Include(r => r.Participants)
            .Where(r => r.Participants.Any(p => p.UserId == userId))
            .ToListAsync();
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        var existingRoom = await _context.Rooms
            .Include(r => r.Participants)
            .FirstOrDefaultAsync(r => r.RoomId == room.RoomId);

        if (existingRoom == null)
        {
            throw new InvalidOperationException($"Room with id {room.RoomId} not found");
        }

        var nameProperty = typeof(Room).GetProperty(nameof(Room.Name));
        if (nameProperty != null)
        {
            nameProperty.SetValue(existingRoom, room.Name);
        }

        var existingParticipantUserIds = existingRoom.Participants.Select(p => p.UserId).ToHashSet();
        var newParticipantUserIds = room.Participants.Select(p => p.UserId).ToHashSet();

        foreach (var userId in existingParticipantUserIds.Except(newParticipantUserIds))
        {
            existingRoom.RemoveParticipant(userId);
        }

        foreach (var userId in newParticipantUserIds.Except(existingParticipantUserIds))
        {
            existingRoom.AddParticipant(userId);
        }

        await _context.SaveChangesAsync();
        return existingRoom;
    }

    public async Task<bool> DeleteAsync(Guid roomId)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null)
        {
            return false;
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }
}

