using System.Threading;
using System.Threading.Tasks;
using Calls.Application.Interfaces;
using Calls.Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Calls.Api.Controllers;

[ApiController]
[Route("api/signaling")]
public class SignalingController : ControllerBase
{
    private readonly ISignalingService _signalingService;

    public SignalingController(ISignalingService signalingService)
    {
        _signalingService = signalingService;
    }

    [HttpPost]
    public async Task<IActionResult> HandleMessage([FromBody] SignalingMessageRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest("Request is required.");

        try
        {
            await _signalingService.HandleAsync(request, cancellationToken);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Room not found.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

