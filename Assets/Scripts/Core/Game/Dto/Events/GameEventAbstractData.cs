using System;
using Core.Game.Mutation;
using Network.Game.Mutation;

namespace Core.Game.Dto
{
    [Serializable]
    public abstract class GameEventAbstractData : IGameEventData
    {
        public GameEventMetadata Metadata;
        
        public abstract void Apply(IGameEventContext context);
    }
}