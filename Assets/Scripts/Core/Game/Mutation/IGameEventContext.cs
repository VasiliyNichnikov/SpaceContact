using Core.Game.Dto;

namespace Network.Game.Mutation
{
    public interface IGameEventContext
    {
        void Execute(GameAggressorSelectedEventData evt);
        
        void Execute(GameDefenderSelectedEventData evt);

        void Execute(GameDestinyCardChangedEventData evt);

        void Execute(GamePlanetToAttackSelectedEventData evt);
    }
}