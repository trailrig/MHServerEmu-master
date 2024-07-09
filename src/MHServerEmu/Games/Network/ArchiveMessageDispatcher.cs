﻿using MHServerEmu.Common.Logging;

namespace MHServerEmu.Games.Network
{
    public class ArchiveMessageDispatcher
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly Game _game;

        public ArchiveMessageDispatcher(Game game)
        {
            _game = game;
        }

        public ulong RegisterMessageHandler(ArchiveMessageHandler handler)
        {
            // Allocate a new replication id if needed
            if (handler.ReplicationId == ArchiveMessageHandler.InvalidReplicationId)
                handler.ReplicationId = _game.CurrentRepId;

            // Check if this repId is already in use
            if (_game.MessageHandlerDict.TryGetValue(handler.ReplicationId, out var registeredHandler))
            {
                if (handler != registeredHandler)
                {
                    Logger.Warn($"Failed to register ArchiveMessageHandler for replicationId {handler.ReplicationId}: this id is already in use by another handler");
                    return ArchiveMessageHandler.InvalidReplicationId;
                }

                return handler.ReplicationId;
            }

            // Register the handler
            _game.MessageHandlerDict.Add(handler.ReplicationId, handler);
            return handler.ReplicationId;
        }

        public void UnregisterMessageHandler(ArchiveMessageHandler handler)
        {
            if (_game.MessageHandlerDict.TryGetValue(handler.ReplicationId, out _) == false)
            {
                Logger.Warn($"Failed to unregister ArchiveMessageHandler for replicationId {handler.ReplicationId}: not found");
                return;
            }

            _game.MessageHandlerDict.Remove(handler.ReplicationId);
        }

        public ArchiveMessageHandler GetMessageHandler(ulong replicationId)
        {
            if (_game.MessageHandlerDict.TryGetValue(replicationId, out var handler) == false)
            {
                Logger.Warn($"Failed to get ArchiveMessageHandler for replicationId {replicationId}: not found");
                return null;
            }

            return handler;
        }

    }
}
