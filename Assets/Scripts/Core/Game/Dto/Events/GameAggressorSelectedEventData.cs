using System;
using Network.Game.Mutation;

namespace Core.Game.Dto
{
    [Serializable]
    public sealed class GameAggressorSelectedEventData : GameEventAbstractData
    {
        public ulong AggressorPlayerId;
        
        public override void Apply(IGameEventContext context) => 
            context.Execute(this);
    }
}