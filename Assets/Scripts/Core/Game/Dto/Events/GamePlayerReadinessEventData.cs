using System;
using Network.Game.Mutation;

namespace Core.Game.Dto
{
    [Serializable]
    public sealed class GamePlayerReadinessEventData : GameEventAbstractData
    {
        public ulong SelectedPlayerId;

        public bool IsPlayerReadyToNextPhase;
        
        public override void Apply(IGameEventContext context) => 
            context.Execute(this);
    }
}