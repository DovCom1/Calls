using Calls.Application.DTO;

namespace Calls.Application.Interfaces;

public interface IRoomService
{
    Task<RoomDto> CreateRoomAsync(string name);
    Task<RoomDto?> GetRoomByIdAsync(Guid roomId);
    Task<List<RoomDto>> GetUserRoomsAsync(Guid userId);
    Task<bool> DeleteRoomAsync(Guid roomId);
}