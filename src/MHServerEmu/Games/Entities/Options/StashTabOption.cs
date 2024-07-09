﻿using System.Text;
using Google.ProtocolBuffers;
using MHServerEmu.Common.Extensions;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;

namespace MHServerEmu.Games.Entities.Options
{
    public class StashTabOption
    {
        public PrototypeId PrototypeId { get; set; }
        public string Name { get; set; }
        public ulong AssetRef { get; set; }
        public int Field2 { get; set; }
        public int Field3 { get; set; }

        public StashTabOption(CodedInputStream stream)
        {
            PrototypeId = stream.ReadPrototypeEnum<Prototype>();
            Name = stream.ReadRawString();
            AssetRef = stream.ReadRawVarint64();
            Field2 = stream.ReadRawInt32();
            Field3 = stream.ReadRawInt32();            
        }

        public StashTabOption(PrototypeId prototypeId, string name, ulong assetRef, int field2, int field3)
        {
            PrototypeId = prototypeId;
            Name = name;
            AssetRef = assetRef;
            Field2 = field2;
            Field3 = field3;
        }

        public void Encode(CodedOutputStream stream)
        {
            stream.WritePrototypeEnum<Prototype>(PrototypeId);
            stream.WriteRawString(Name);
            stream.WriteRawVarint64(AssetRef);
            stream.WriteRawInt32(Field2);
            stream.WriteRawInt32(Field3);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"PrototypeId: {GameDatabase.GetPrototypeName(PrototypeId)}");
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"AssetRef: {AssetRef}");
            sb.AppendLine($"Field2: 0x{Field2}");
            sb.AppendLine($"Field3: 0x{Field3}");
            return sb.ToString();
        }
    }
}
