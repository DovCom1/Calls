using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Calls.Application.Signaling;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SignalingMessageType
{
    [EnumMember(Value = "room_join")]
    RoomJoin,

    [EnumMember(Value = "room_invite")]
    RoomInvite,

    [EnumMember(Value = "room_leave")]
    RoomLeave,

    [EnumMember(Value = "webrtc_offer")]
    WebrtcOffer,

    [EnumMember(Value = "webrtc_answer")]
    WebrtcAnswer,

    [EnumMember(Value = "webrtc_ice_candidate")]
    WebrtcIceCandidate,

    [EnumMember(Value = "emotion_send")]
    EmotionSend,

    [EnumMember(Value = "error")]
    Error
}

