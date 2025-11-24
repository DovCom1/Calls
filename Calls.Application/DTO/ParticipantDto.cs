namespace Calls.Application.DTO;

public class ParticipantDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}