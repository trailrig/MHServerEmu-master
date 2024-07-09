﻿using MHServerEmu.Common.Helpers;
using MHServerEmu.Common.Logging;

namespace MHServerEmu.Games.GameData.LiveTuning
{
    public static class LiveTuningManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        public static bool IsInitialized { get; }
        public static LiveTuningData LiveTuningData { get; }

        // TODO: Make LiveTuningData per game and have games copy it from the manager

        static LiveTuningManager()
        {
            LiveTuningData = new(Path.Combine(FileHelper.AssetsDirectory, "LiveTuningData.json"));
            Logger.Info($"Loaded {LiveTuningData.Count} live tuning settings");
            IsInitialized = true;
        }
    }
}
