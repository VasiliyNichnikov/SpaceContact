using System;
using Network.Game.Mutation;

namespace Core.Game.Dto
{
    [Serializable]
    public sealed class GamePlanetToAttackSelectedEventData : GameEventAbstractData
    {
        public int PlanetId;
        
        public ulong InitiatedByPlayerId;
        
        public override void Apply(IGameEventContext context) => 
            context.Execute(this);
    }
}