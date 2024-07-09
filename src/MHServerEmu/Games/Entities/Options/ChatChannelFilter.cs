﻿using System.Text;
using Google.ProtocolBuffers;
using Gazillion;
using MHServerEmu.Common.Encoders;
using MHServerEmu.Common.Extensions;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;

namespace MHServerEmu.Games.Entities.Options
{
    public class ChatChannelFilter
    {
        public PrototypeId ChannelProtoId { get; set; }
        public bool IsSubscribed { get; set; }

        public ChatChannelFilter(CodedInputStream stream, BoolDecoder boolDecoder)
        {
            ChannelProtoId = stream.ReadPrototypeEnum<Prototype>();
            IsSubscribed = boolDecoder.ReadBool(stream);
        }

        public ChatChannelFilter(PrototypeId channelProtoId, bool isSubscribed)
        {
            ChannelProtoId = channelProtoId;
            IsSubscribed = isSubscribed;
        }

        public ChatChannelFilter(NetStructChatChannelFilterState netStruct)
        {
            ChannelProtoId = (PrototypeId)netStruct.ChannelProtoId;
            IsSubscribed = netStruct.IsSubscribed;
        }

        public void Encode(CodedOutputStream stream, BoolEncoder boolEncoder)
        {
            stream.WritePrototypeEnum<Prototype>(ChannelProtoId);
            boolEncoder.WriteBuffer(stream);   // IsSubscribed
        }

        public NetStructChatChannelFilterState ToNetStruct() => NetStructChatChannelFilterState.CreateBuilder().SetChannelProtoId((ulong)ChannelProtoId).SetIsSubscribed(IsSubscribed).Build();

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"ChannelProtoId: {GameDatabase.GetPrototypeName(ChannelProtoId)}");
            sb.AppendLine($"IsSubscribed: {IsSubscribed}");
            return sb.ToString();
        }
    }
}
