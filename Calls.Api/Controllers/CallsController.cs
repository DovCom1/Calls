using Microsoft.AspNetCore.Mvc;
using Calls.Application.DTO;
using Calls.Application.Interfaces;
using Calls.Application.Requests;

namespace Calls.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CallsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public CallsController(IRoomService roomService)
    {
        _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] CreateRoomRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Room name is required.");

        var room = await _roomService.CreateRoomAsync(request.Name);
        return CreatedAtAction(nameof(GetRoom), new { roomId = room.RoomId }, room);
    }

    [HttpGet("{roomId:guid}")]
    public async Task<ActionResult<RoomDto>> GetRoom(Guid roomId)
    {
        var room = await _roomService.GetRoomByIdAsync(roomId);
        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<RoomDto>>> GetUserRooms(Guid userId)
    {
        var rooms = await _roomService.GetUserRoomsAsync(userId);
        return Ok(rooms);
    }

    [HttpDelete("{roomId:guid}")]
    public async Task<IActionResult> DeleteRoom(Guid roomId)
    {
        var deleted = await _roomService.DeleteRoomAsync(roomId);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}