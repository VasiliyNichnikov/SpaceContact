using System;
using Network.Game.Mutation;

namespace Core.Game.Dto
{
    [Serializable]
    public sealed class GameDefenderSelectedEventData : GameEventAbstractData
    {
        public ulong DefenderPlayerId;
        
        public override void Apply(IGameEventContext context) => 
            context.Execute(this);
    }
}