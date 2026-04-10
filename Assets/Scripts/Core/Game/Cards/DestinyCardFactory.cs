using Core.Game.Dto.States.Cards;
using Core.Game.Players;
using Logs;

namespace Core.Game.Cards
{
    public sealed class DestinyCardFactory
    {
        private readonly GamePlayersRegistry _playersRegistry;
        
        public DestinyCardFactory(GamePlayersRegistry playersRegistry)
        {
            _playersRegistry = playersRegistry;
        }
        
        public IDestinyCard Create(DestinyCardData stateData)
        {
            if (stateData.IsColorCard)
            {
                var targetPlayer = _playersRegistry.GetPlayerById(stateData.SelectedPlayerId);
                
                return new GamePlayerColorDestinyCard(targetPlayer);
            }

            Logger.Warning($"{nameof(DestinyCardFactory)}.{nameof(Create)}: card is not supported.");
            return ErrorDestinyCard.Instance;
        }
    }
}