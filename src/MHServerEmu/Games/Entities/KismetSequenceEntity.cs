﻿using Google.ProtocolBuffers;

namespace MHServerEmu.Games.Entities
{
    // KismetSequenceEntity doesn't contain any data of its own, but probably contains behavior
    public class KismetSequenceEntity : WorldEntity
    {
        public KismetSequenceEntity(EntityBaseData baseData, ByteString archiveData) : base(baseData, archiveData) { }

        public KismetSequenceEntity(EntityBaseData baseData) : base(baseData) { }
    }
}
