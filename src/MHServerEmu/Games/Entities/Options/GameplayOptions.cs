﻿using System.Text;
using Google.ProtocolBuffers;
using Gazillion;
using MHServerEmu.Common.Encoders;
using MHServerEmu.Common.Extensions;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.Loot;

namespace MHServerEmu.Games.Entities.Options
{
    public enum GameplayOptionSetting
    {
        AutoPartyEnabled,
        Option1,
        DisableHeroSynergyBonusXP,
        Option3,
        EnableVaporizeCredits,
        Option5,
        ShowPlayerFloatingDamageNumbers,
        ShowEnemyFloatingDamageNumbers,
        ShowExperienceFloatingNumbers,
        ShowBossIndicator,
        ShowPartyMemberArrows,
        MusicLevel,
        SfxLevel,
        ShowMovieSubtitles,
        MicLevel,
        SpeakerLevel,
        GammaLevel,
        ShowPlayerHealingNumbers,
        ShowPlayerIndicator
    }

    public class GameplayOptions
    {
        public ChatChannelFilter[] ChatChannelFilters { get; set; } // ChatChannelFilterMap
        public PrototypeId[] ChatTabChannels { get; set; }                // ChatTabState
        public long[] OptionSettings { get; set; }
        public ArmorRarityVaporizeThreshold[] ArmorRarityVaporizeThresholds { get; set; }

        public GameplayOptions(CodedInputStream stream, BoolDecoder boolDecoder)
        {
            ChatChannelFilters = new ChatChannelFilter[stream.ReadRawVarint64()];
            for (int i = 0; i < ChatChannelFilters.Length; i++)
                ChatChannelFilters[i] = new(stream, boolDecoder);

            ChatTabChannels = new PrototypeId[stream.ReadRawVarint64()];
            for (int i = 0; i < ChatTabChannels.Length; i++)
                ChatTabChannels[i] = stream.ReadPrototypeEnum<Prototype>();

            OptionSettings = new long[stream.ReadRawVarint64()];
            for (int i = 0; i < OptionSettings.Length; i++)
                OptionSettings[i] = (long)stream.ReadRawVarint64();

            ArmorRarityVaporizeThresholds = new ArmorRarityVaporizeThreshold[stream.ReadRawVarint64()];
            for (int i = 0; i < ArmorRarityVaporizeThresholds.Length; i++)
                ArmorRarityVaporizeThresholds[i] = new(stream);
        }

        public GameplayOptions(ChatChannelFilter[] chatChannelFilters, PrototypeId[] chatTabChannels, long[] optionSettings, ArmorRarityVaporizeThreshold[] armorRarityVaporizeThresholds)
        {
            ChatChannelFilters = chatChannelFilters;
            ChatTabChannels = chatTabChannels;
            OptionSettings = optionSettings;
            ArmorRarityVaporizeThresholds = armorRarityVaporizeThresholds;
        }

        public GameplayOptions(NetStructGameplayOptions netStruct)
        {
            ChatChannelFilters = netStruct.ChatChannelFiltersMapList.Select(filter => new ChatChannelFilter(filter)).ToArray();
            ChatTabChannels = netStruct.ChatTabChannelsArrayList.Select(channel => (PrototypeId)channel.ChannelProtoId).ToArray();
            OptionSettings = netStruct.OptionSettingsList.Select(setting => (long)setting).ToArray();

            ArmorRarityVaporizeThresholds = new ArmorRarityVaporizeThreshold[netStruct.ArmorRarityVaporizeThresholdProtoIdCount];
            for (int i = 0; i < ArmorRarityVaporizeThresholds.Length; i++)
                ArmorRarityVaporizeThresholds[i] = new((EquipmentInvUISlot)(i + 1), (PrototypeId)netStruct.ArmorRarityVaporizeThresholdProtoIdList[i]);
        }

        public void EncodeBools(BoolEncoder boolEncoder)
        {
            foreach (ChatChannelFilter filter in ChatChannelFilters)
                boolEncoder.EncodeBool(filter.IsSubscribed);
        }

        public void Encode(CodedOutputStream stream, BoolEncoder boolEncoder)
        {
            stream.WriteRawVarint64((ulong)ChatChannelFilters.Length);
            foreach (ChatChannelFilter filter in ChatChannelFilters) filter.Encode(stream, boolEncoder);

            stream.WriteRawVarint64((ulong)ChatTabChannels.Length);
            foreach (PrototypeId channel in ChatTabChannels) stream.WritePrototypeEnum<Prototype>(channel);

            stream.WriteRawVarint64((ulong)OptionSettings.Length);
            foreach (long setting in OptionSettings) stream.WriteRawVarint64((ulong)setting);

            stream.WriteRawVarint64((ulong)ArmorRarityVaporizeThresholds.Length);
            foreach (ArmorRarityVaporizeThreshold threshold in ArmorRarityVaporizeThresholds) threshold.Encode(stream);
        }

        public NetStructGameplayOptions ToNetStruct()
        {
            return NetStructGameplayOptions.CreateBuilder()
                .AddRangeOptionSettings(OptionSettings.Select(setting => (ulong)setting))
                .AddRangeChatChannelFiltersMap(ChatChannelFilters.Select(filter => filter.ToNetStruct()))
                .AddRangeChatTabChannelsArray(ChatTabChannels.Select(channel => NetStructChatTabState.CreateBuilder().SetChannelProtoId((ulong)channel).Build()))
                .AddRangeArmorRarityVaporizeThresholdProtoId(ArmorRarityVaporizeThresholds.Select(threshold => (ulong)threshold.RarityPrototypeId))
                .Build();
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < ChatChannelFilters.Length; i++) sb.AppendLine($"ChatChannelFilter{i}: {ChatChannelFilters[i]}");
            for (int i = 0; i < ChatTabChannels.Length; i++) sb.AppendLine($"ChatTabChannel{i}: {GameDatabase.GetPrototypeName(ChatTabChannels[i])}");
            for (int i = 0; i < OptionSettings.Length; i++) sb.AppendLine($"OptionSetting{i} ({(GameplayOptionSetting)i}): {OptionSettings[i]}");
            for (int i = 0; i < ArmorRarityVaporizeThresholds.Length; i++) sb.AppendLine($"ArmorRarityVaporizeThreshold{i}: {ArmorRarityVaporizeThresholds[i]}");
            return sb.ToString();
        }
    }
}
