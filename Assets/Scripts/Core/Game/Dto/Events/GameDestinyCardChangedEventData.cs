using System;
using Core.Game.Dto.States.Cards;
using Network.Game.Mutation;

namespace Core.Game.Dto
{
    [Serializable]
    public sealed class GameDestinyCardChangedEventData : GameEventAbstractData
    {
        public DestinyCardData DestinyCard;
        
        public override void Apply(IGameEventContext context) => 
            context.Execute(this);
    }
}