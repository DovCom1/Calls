using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calls.Application.Interfaces;
using Calls.Application.Interfaces.External;
using Calls.Application.Notifications;
using Calls.Application.Requests;
using Calls.Application.Signaling;
using Calls.Domain.Rooms;
using Microsoft.Extensions.Logging;

namespace Calls.Application.Services;

public class SignalingService(
    IRoomRepository roomRepository,
    IChangerNotifierClient changerNotifierClient,
    ILogger<SignalingService> logger) : ISignalingService
{

    public async Task HandleAsync(SignalingMessageRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        switch (request.Type)
        {
            case SignalingMessageType.RoomJoin:
                await HandleRoomJoinAsync(request, cancellationToken);
                break;
            case SignalingMessageType.RoomInvite:
                await ForwardToRecipientAsync(request, new[] { request.To!.Value }, cancellationToken);
                break;
            case SignalingMessageType.RoomLeave:
                await HandleRoomLeaveAsync(request, cancellationToken);
                break;
            case SignalingMessageType.WebrtcOffer:
            case SignalingMessageType.WebrtcAnswer:
            case SignalingMessageType.WebrtcIceCandidate:
                await ForwardToRecipientAsync(request, new[] { request.To!.Value }, cancellationToken);
                break;
            case SignalingMessageType.EmotionSend:
                await BroadcastToRoomAsync(request, includeSender: true, cancellationToken);
                break;
            case SignalingMessageType.Error:
                await ForwardToRecipientAsync(request, new[] { request.To!.Value }, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(request.Type), request.Type, "Unknown signaling message type.");
        }
    }

    private static void ValidateRequest(SignalingMessageRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Payload == null)
            throw new ArgumentException("Payload is required.", nameof(request.Payload));

        if (request.From == Guid.Empty)
            throw new ArgumentException("Sender is required.", nameof(request.From));

        if (RequiresRecipient(request.Type) && (!request.To.HasValue || request.To == Guid.Empty))
            throw new ArgumentException("Recipient is required for the selected message type.", nameof(request.To));
    }

    private static bool RequiresRecipient(SignalingMessageType type) =>
        type switch
        {
            SignalingMessageType.RoomInvite or
            SignalingMessageType.WebrtcOffer or
            SignalingMessageType.WebrtcAnswer or
            SignalingMessageType.WebrtcIceCandidate or
            SignalingMessageType.Error => true,
            _ => false
        };

    private async Task HandleRoomJoinAsync(SignalingMessageRequest request, CancellationToken cancellationToken)
    {
        var room = await GetRoomOrThrowAsync(request, cancellationToken);
        var participantAdded = room.AddParticipant(request.From);
        if (participantAdded)
            await roomRepository.UpdateAsync(room);

        var recipients = room.Participants
            .Select(p => p.UserId)
            .Where(userId => userId != request.From)
            .ToList();

        await ForwardToRecipientAsync(request, recipients, cancellationToken, room);
    }

    private async Task HandleRoomLeaveAsync(SignalingMessageRequest request, CancellationToken cancellationToken)
    {
        var room = await GetRoomOrThrowAsync(request, cancellationToken);
        var removed = room.RemoveParticipant(request.From);
        if (removed)
            await roomRepository.UpdateAsync(room);

        var recipients = room.Participants
            .Select(p => p.UserId)
            .Where(userId => userId != request.From)
            .ToList();

            await ForwardToRecipientAsync(request, recipients, cancellationToken, room);
    }

    private async Task BroadcastToRoomAsync(SignalingMessageRequest request, bool includeSender, CancellationToken cancellationToken)
    {
        var room = await GetRoomOrThrowAsync(request, cancellationToken);
        var recipients = room.Participants
            .Select(p => p.UserId)
            .Where(userId => includeSender || userId != request.From)
            .ToList();

        await ForwardToRecipientAsync(request, recipients, cancellationToken, room);
    }

    private async Task ForwardToRecipientAsync(
        SignalingMessageRequest request,
        IEnumerable<Guid> recipients,
        CancellationToken cancellationToken,
        Room? room = null)
    {
        var envelopePipelines = recipients
            .Distinct()
            .Select(recipient => BuildEnvelope(recipient, request, room));

        foreach (var envelope in envelopePipelines)
        {
            await changerNotifierClient.NotifyAsync(envelope, cancellationToken);
        }
    }

    private ChangerNotifierEnvelope BuildEnvelope(Guid recipientId, SignalingMessageRequest request, Room? room)
    {
        var clonedMessage = CloneRequest(request, room);
        clonedMessage.To = recipientId;

        return new ChangerNotifierEnvelope
        {
            RecipientId = recipientId,
            Message = clonedMessage
        };
    }

    private static SignalingMessageRequest CloneRequest(SignalingMessageRequest source, Room? room)
    {
        return new SignalingMessageRequest
        {
            Type = source.Type,
            From = source.From,
            To = source.To,
            Payload = new SignalingPayload
            {
                RoomInfo = room != null ? MapRoomInfo(room) : source.Payload.RoomInfo,
                Error = source.Payload.Error,
                Sdp = source.Payload.Sdp,
                IceCandidate = source.Payload.IceCandidate,
                Emotion = source.Payload.Emotion
            }
        };
    }

    private static SignalingRoomInfo MapRoomInfo(Room room)
    {
        return new SignalingRoomInfo
        {
            RoomId = room.RoomId,
            Name = room.Name,
            Participants = room.Participants.Select(p => p.UserId).ToList()
        };
    }

    private async Task<Room> GetRoomOrThrowAsync(SignalingMessageRequest request, CancellationToken cancellationToken)
    {
        var roomId = request.Payload.RoomInfo?.RoomId;
        if (roomId == null || roomId == Guid.Empty)
            throw new ArgumentException("Room identifier is required in payload.", nameof(request.Payload.RoomInfo));

        var room = await roomRepository.GetByIdAsync(roomId.Value);
        if (room == null)
        {
            logger.LogWarning("Room {RoomId} was not found for signaling message {Type}.", roomId, request.Type);
            throw new KeyNotFoundException($"Room {roomId} not found.");
        }

        return room;
    }
}

