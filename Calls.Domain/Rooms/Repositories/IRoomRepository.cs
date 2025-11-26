using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calls.Domain.Rooms;

public interface IRoomRepository
{
    Task<Room> AddAsync(Room room);
    Task<Room?> GetByIdAsync(Guid roomId);
    Task<List<Room>> GetByUserIdAsync(Guid userId);
    Task<Room> UpdateAsync(Room room);
    Task<bool> DeleteAsync(Guid roomId);
}

