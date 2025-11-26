using System.Linq;
using Calls.Application.DTO;
using Calls.Application.Interfaces;
using Calls.Domain.Rooms;

namespace Calls.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<RoomDto> CreateRoomAsync(string name)
    {
        var room = Room.Create(name);
        var created = await _roomRepository.AddAsync(room);
        return MapToDto(created);
    }

    public async Task<RoomDto?> GetRoomByIdAsync(Guid roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        return room is null ? null : MapToDto(room);
    }

    public async Task<List<RoomDto>> GetUserRoomsAsync(Guid userId)
    {
        var rooms = await _roomRepository.GetByUserIdAsync(userId);
        return rooms.Select(MapToDto).ToList();
    }

    public Task<bool> DeleteRoomAsync(Guid roomId)
    {
        return _roomRepository.DeleteAsync(roomId);
    }

    private static RoomDto MapToDto(Room room)
    {
        return new RoomDto
        {
            RoomId = room.RoomId,
            Name = room.Name,
            Participants = room.Participants
                .Select(p => new ParticipantDto
                {
                    UserId = p.UserId,
                    DisplayName = string.Empty
                })
                .ToList()
        };
    }
}