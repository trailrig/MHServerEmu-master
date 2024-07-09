﻿using System.Text;
using Google.ProtocolBuffers;
using MHServerEmu.Common.Encoders;
using MHServerEmu.Games.Missions;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.UI;

namespace MHServerEmu.Games.Powers
{
    public class RegionArchive
    {
        public AoiNetworkPolicyValues ReplicationPolicy { get; set; }
        public ReplicatedPropertyCollection PropertyCollection { get; set; }
        public MissionManager MissionManager { get; set; }
        public UIDataProvider UIDataProvider { get; set; }
        public ObjectiveGraph ObjectiveGraph { get; set; }

        public RegionArchive(ByteString data)
        {
            CodedInputStream stream = CodedInputStream.CreateInstance(data.ToByteArray());
            BoolDecoder boolDecoder = new();

            ReplicationPolicy = (AoiNetworkPolicyValues)stream.ReadRawVarint32();
            PropertyCollection = new(stream);
            MissionManager = new(stream, boolDecoder);
            UIDataProvider = new(stream, boolDecoder);
            ObjectiveGraph = new(stream);
        }

        public RegionArchive() { }

        public ByteString Serialize()
        {
            using (MemoryStream ms = new())
            {
                CodedOutputStream cos = CodedOutputStream.CreateInstance(ms);

                // Prepare bool encoder
                BoolEncoder boolEncoder = new();
                MissionManager.EncodeBools(boolEncoder);
                UIDataProvider.EncodeBools(boolEncoder);
                boolEncoder.Cook();

                // Encode
                cos.WriteRawVarint32((uint)ReplicationPolicy);
                PropertyCollection.Encode(cos);
                MissionManager.Encode(cos, boolEncoder);
                UIDataProvider.Encode(cos, boolEncoder);
                ObjectiveGraph.Encode(cos);

                cos.Flush();
                return ByteString.CopyFrom(ms.ToArray());
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"ReplicationPolicy: {ReplicationPolicy}");
            sb.AppendLine($"PropertyCollection: {PropertyCollection}");
            sb.AppendLine($"MissionManager: {MissionManager}");
            sb.AppendLine($"UIDataProvider: {UIDataProvider}");
            sb.AppendLine($"ObjectiveGraph: {ObjectiveGraph}");

            return sb.ToString();
        }
    }
}
