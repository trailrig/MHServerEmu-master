﻿using System.Text;
using Google.ProtocolBuffers;
using MHServerEmu.Common.Encoders;
using MHServerEmu.Common.Extensions;
using MHServerEmu.Games.Achievements;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities.Options;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Missions;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.Social;

namespace MHServerEmu.Games.Entities
{
    public class Player : Entity
    {
        public MissionManager MissionManager { get; set; }
        public ReplicatedPropertyCollection AvatarPropertyCollection { get; set; }
        public ulong ShardId { get; set; }
        public ReplicatedVariable<string> Name { get; set; }
        public ulong ConsoleAccountId1 { get; set; }
        public ulong ConsoleAccountId2 { get; set; }
        public ReplicatedVariable<string> UnkName { get; set; }
        public ulong MatchQueueStatus { get; set; }
        public bool EmailVerified { get; set; }
        public ulong AccountCreationTimestamp { get; set; }
        public ReplicatedVariable<ulong> PartyId { get; set; }
        public string UnknownString { get; set; }
        public bool HasGuildInfo { get; set; }
        public GuildMemberReplicationRuntimeInfo GuildInfo { get; set; }
        public bool HasCommunity { get; set; }
        public Community Community { get; set; }
        public bool UnkBool { get; set; }
        public PrototypeId[] StashInventories { get; set; }
        public uint[] AvailableBadges { get; set; }
        public GameplayOptions GameplayOptions { get; set; }
        public AchievementState[] AchievementStates { get; set; }
        public StashTabOption[] StashTabOptions { get; set; }

        public Player(EntityBaseData baseData, ByteString archiveData) : base(baseData, archiveData) { }

        // note: this is ugly
        public Player(EntityBaseData baseData, AoiNetworkPolicyValues replicationPolicy, ReplicatedPropertyCollection propertyCollection,
            MissionManager missionManager, ReplicatedPropertyCollection avatarProperties,
            ulong shardId, ReplicatedVariable<string> playerName, ReplicatedVariable<string> unkName,
            ulong matchQueueStatus, bool emailVerified, ulong accountCreationTimestamp, ReplicatedVariable<ulong> partyId,
            Community community, bool unkBool, PrototypeId[] stashInventories, uint[] availableBadges,
            GameplayOptions gameplayOptions, AchievementState[] achievementStates, StashTabOption[] stashTabOptions) : base(baseData)
        {
            ReplicationPolicy = replicationPolicy;
            PropertyCollection = propertyCollection;

            MissionManager = missionManager;
            AvatarPropertyCollection = avatarProperties;
            ShardId = shardId;
            Name = playerName;
            ConsoleAccountId1 = 0;
            ConsoleAccountId2 = 0;
            UnkName = unkName;
            MatchQueueStatus = matchQueueStatus;
            EmailVerified = emailVerified;
            AccountCreationTimestamp = accountCreationTimestamp;
            PartyId = partyId;
            Community = community;
            UnkBool = unkBool;
            StashInventories = stashInventories;
            AvailableBadges = availableBadges;
            GameplayOptions = gameplayOptions;
            AchievementStates = achievementStates;
            StashTabOptions = stashTabOptions;
        }

        protected override void Decode(CodedInputStream stream)
        {
            base.Decode(stream);

            BoolDecoder boolDecoder = new();

            MissionManager = new(stream, boolDecoder);
            AvatarPropertyCollection = new(stream);

            ShardId = stream.ReadRawVarint64();
            Name = new(stream);
            ConsoleAccountId1 = stream.ReadRawVarint64();
            ConsoleAccountId2 = stream.ReadRawVarint64();
            UnkName = new(stream);
            MatchQueueStatus = stream.ReadRawVarint64();
            EmailVerified = boolDecoder.ReadBool(stream);
            AccountCreationTimestamp = stream.ReadRawVarint64();

            PartyId = new(stream);

            HasGuildInfo = boolDecoder.ReadBool(stream);
            if (HasGuildInfo) GuildInfo = new(stream);      // GuildMember::SerializeReplicationRuntimeInfo

            UnknownString = stream.ReadRawString();

            HasCommunity = boolDecoder.ReadBool(stream);
            if (HasCommunity) Community = new(stream);

            UnkBool = boolDecoder.ReadBool(stream);

            StashInventories = new PrototypeId[stream.ReadRawVarint64()];
            for (int i = 0; i < StashInventories.Length; i++)
                StashInventories[i] = stream.ReadPrototypeEnum<Prototype>();

            AvailableBadges = new uint[stream.ReadRawVarint64()];
            for (int i = 0; i < AvailableBadges.Length; i++) AvailableBadges[i] = stream.ReadRawVarint32();

            GameplayOptions = new(stream, boolDecoder);

            AchievementStates = new AchievementState[stream.ReadRawVarint64()];
            for (int i = 0; i < AchievementStates.Length; i++)
                AchievementStates[i] = new(stream);

            StashTabOptions = new StashTabOption[stream.ReadRawVarint64()];
            for (int i = 0; i < StashTabOptions.Length; i++)
                StashTabOptions[i] = new(stream);
        }

        public override void Encode(CodedOutputStream stream)
        {
            base.Encode(stream);

            // Prepare bool encoder
            BoolEncoder boolEncoder = new();

            MissionManager.EncodeBools(boolEncoder);

            boolEncoder.EncodeBool(EmailVerified);
            boolEncoder.EncodeBool(HasGuildInfo);
            boolEncoder.EncodeBool(HasCommunity);
            boolEncoder.EncodeBool(UnkBool);

            GameplayOptions.EncodeBools(boolEncoder);

            boolEncoder.Cook();

            // Encode
            MissionManager.Encode(stream, boolEncoder);
            AvatarPropertyCollection.Encode(stream);

            stream.WriteRawVarint64(ShardId);
            Name.Encode(stream);
            stream.WriteRawVarint64(ConsoleAccountId1);
            stream.WriteRawVarint64(ConsoleAccountId2);
            UnkName.Encode(stream);
            stream.WriteRawVarint64(MatchQueueStatus);
            boolEncoder.WriteBuffer(stream);   // EmailVerified
            stream.WriteRawVarint64(AccountCreationTimestamp);

            PartyId.Encode(stream);

            boolEncoder.WriteBuffer(stream);   // HasGuildInfo
            if (HasGuildInfo) GuildInfo.Encode(stream);

            stream.WriteRawString(UnknownString);

            boolEncoder.WriteBuffer(stream);   // HasCommunity
            if (HasCommunity) Community.Encode(stream);

            boolEncoder.WriteBuffer(stream);   // UnkBool

            stream.WriteRawVarint64((ulong)StashInventories.Length);
            foreach (PrototypeId stashInventory in StashInventories) stream.WritePrototypeEnum<Prototype>(stashInventory);

            stream.WriteRawVarint64((ulong)AvailableBadges.Length);
            foreach (uint badge in AvailableBadges) stream.WriteRawVarint32(badge);

            GameplayOptions.Encode(stream, boolEncoder);

            stream.WriteRawVarint64((ulong)AchievementStates.Length);
            foreach (AchievementState state in AchievementStates) state.Encode(stream);

            stream.WriteRawVarint64((ulong)StashTabOptions.Length);
            foreach (StashTabOption option in StashTabOptions) option.Encode(stream);
        }

        protected override void BuildString(StringBuilder sb)
        {
            base.BuildString(sb);

            sb.AppendLine($"MissionManager: {MissionManager}");
            sb.AppendLine($"AvatarPropertyCollection: {AvatarPropertyCollection}");
            sb.AppendLine($"ShardId: {ShardId}");
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"ConsoleAccountId1: 0x{ConsoleAccountId1:X}");
            sb.AppendLine($"ConsoleAccountId2: 0x{ConsoleAccountId2:X}");
            sb.AppendLine($"UnkName: {UnkName}");
            sb.AppendLine($"MatchQueueStatus: 0x{MatchQueueStatus:X}");
            sb.AppendLine($"EmailVerified: {EmailVerified}");
            sb.AppendLine($"AccountCreationTimestamp: 0x{AccountCreationTimestamp:X}");
            sb.AppendLine($"PartyId: {PartyId}");
            sb.AppendLine($"HasGuildInfo: {HasGuildInfo}");
            sb.AppendLine($"GuildInfo: {GuildInfo}");
            sb.AppendLine($"UnknownString: {UnknownString}");
            sb.AppendLine($"HasCommunity: {HasCommunity}");
            sb.AppendLine($"Community: {Community}");
            sb.AppendLine($"UnkBool: {UnkBool}");
            for (int i = 0; i < StashInventories.Length; i++) sb.AppendLine($"StashInventory{i}: {GameDatabase.GetPrototypeName(StashInventories[i])}");
            for (int i = 0; i < AvailableBadges.Length; i++) sb.AppendLine($"AvailableBadge{i}: 0x{AvailableBadges[i]:X}");
            sb.AppendLine($"GameplayOptions: {GameplayOptions}");
            for (int i = 0; i < AchievementStates.Length; i++) sb.AppendLine($"AchievementState{i}: {AchievementStates[i]}");
            for (int i = 0; i < StashTabOptions.Length; i++) sb.AppendLine($"StashTabOption{i}: {StashTabOptions[i]}");
        }
    }
}
