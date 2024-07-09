﻿using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Common.Config.Sections
{
    public class DefaultPlayerDataConfig
    {
        private const string Section = "DefaultPlayerData";

        public string PlayerName { get; }
        public RegionPrototypeId StartingRegion { get; }
        public AvatarPrototypeId StartingAvatar { get; }

        public DefaultPlayerDataConfig(IniFile configFile)
        {
            PlayerName = configFile.ReadString(Section, nameof(PlayerName));

            // StartingRegion
            string startingRegion = configFile.ReadString(Section, nameof(StartingRegion));

            if (Enum.TryParse(typeof(RegionPrototypeId), startingRegion, out object regionPrototypeEnum))
                StartingRegion = (RegionPrototypeId)regionPrototypeEnum;
            else
                StartingRegion = RegionPrototypeId.NPEAvengersTowerHUBRegion;

            // StartingHero
            string startingAvatar = configFile.ReadString(Section, nameof(StartingAvatar));

            if (Enum.TryParse(typeof(AvatarPrototypeId), startingAvatar, out object avatarEntityEnum))
                StartingAvatar = (AvatarPrototypeId)avatarEntityEnum;
            else
                StartingAvatar = AvatarPrototypeId.BlackCat;
        }
    }
}
