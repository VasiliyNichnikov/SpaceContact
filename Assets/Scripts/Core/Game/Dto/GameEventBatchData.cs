using System;
using Core.Game.Events;

namespace Core.Game.Dto
{
    [Serializable]
    public sealed class GameEventBatchData
    {
        public long Revision;

        public IGameEvent[] Events = Array.Empty<IGameEvent>();
    }
}