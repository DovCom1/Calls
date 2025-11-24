using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Calls.Application.DTO;
using Calls.Application.Requests;

namespace Calls.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CallsController : ControllerBase
{
    // Предполагается, что вы внедрите IRoomService через DI
    private readonly IRoomService _roomService;

    public CallsController(IRoomService roomService)
    {
        roomService = roomService;
    }

    /// <summary>
    /// Создание новой комнаты
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] CreateRoomRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Room name is required.");

        var room = await _roomService.CreateRoomAsync(request.Name);
        return CreatedAtAction(nameof(GetRoom), new { roomId = room.RoomId }, room);
    }

    /// <summary>
    /// Получение информации о комнате по ID
    /// </summary>
    [HttpGet("{roomId:guid}")]
    public async Task<ActionResult<RoomDto>> GetRoom(Guid roomId)
    {
        var room = await _roomService.GetRoomByIdAsync(roomId);
        if (room == null)
            return NotFound();

        return Ok(room);
    }

    /// <summary>
    /// Получение списка комнат пользователя
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<RoomDto>>> GetUserRooms(Guid userId)
    {
        var rooms = await _roomService.GetUserRoomsAsync(userId);
        return Ok(rooms);
    }

    /// <summary>
    /// Удаление комнаты
    /// </summary>
    [HttpDelete("{roomId:guid}")]
    public async Task<IActionResult> DeleteRoom(Guid roomId)
    {
        var deleted = await _roomService.DeleteRoomAsync(roomId);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}