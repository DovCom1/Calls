using System.Threading;
using System.Threading.Tasks;
using Calls.Application.Interfaces;
using Calls.Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Calls.Api.Controllers;

[ApiController]
[Route("api/signaling")]
public class SignalingController(ISignalingService signalingService) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> HandleMessage([FromBody] SignalingMessageRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest("Request is required.");

        try
        {
            await signalingService.HandleAsync(request, cancellationToken);
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

