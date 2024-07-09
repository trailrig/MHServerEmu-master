﻿using Google.ProtocolBuffers;

namespace MHServerEmu.Games.Entities
{
    // Missile doesn't contain any data of its own, but probably contains behavior
    public class Missile : Agent
    {
        public Missile(EntityBaseData baseData, ByteString archiveData) : base(baseData, archiveData) { }

        public Missile(EntityBaseData baseData) : base(baseData) { }
    }
}
