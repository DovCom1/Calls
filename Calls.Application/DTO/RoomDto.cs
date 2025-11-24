namespace Calls.Application.DTO;

public class RoomDto
{
    public Guid RoomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ParticipantDto> Participants { get; set; } = new();
}
