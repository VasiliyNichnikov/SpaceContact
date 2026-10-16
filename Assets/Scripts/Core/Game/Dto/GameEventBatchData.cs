using System;
using Core.Game.Events;
using Core.Game.Validation;

namespace Core.Game.Dto
{
    [Serializable]
    public sealed class GameEventBatchData
    {
        [StateRequired]
        public long Revision;

        [StateRequired]
        public IGameEvent[] Events = Array.Empty<IGameEvent>();
    }
}