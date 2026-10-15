using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto;
using Core.Game.Visibility;

namespace Core.Game.Events
{
    public sealed class GameEventBatchFactory
    {
        internal Dictionary<ulong, GameEventBatchData> CreateBatches(GameContext context, long revision, IReadOnlyList<IGameEvent> domainEvents)
        {
            var result = new Dictionary<ulong, GameEventBatchData>();

            foreach (var player in context.Players.All)
            {
                var clientEvents = domainEvents
                    .SelectMany(gameEvent => gameEvent.ToClientEvents(context, player.Id))
                    .Select(gameEvent => StateProjection.ToClient(gameEvent, player.Id))
                    .ToArray();

                result.Add(player.Id, new GameEventBatchData
                {
                    Revision = revision,
                    Events = clientEvents
                });
            }

            return result;
        }
    }
}
