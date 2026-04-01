using Core.Game.Dto.States.Cards;
using Core.Game.Players;
using Logs;

namespace Core.Game.Cards
{
    public class DestinyCardFactory
    {
        private readonly GamePlayersRegistry _playersRegistry;
        
        public DestinyCardFactory(GamePlayersRegistry playersRegistry)
        {
            _playersRegistry = playersRegistry;
        }
        
        public IDestinyCard Create(DestinyCardStateData stateData)
        {
            if (stateData.IsColorCard)
            {
                var gamePlayer = _playersRegistry.GetPlayerById(stateData.SelectedPlayerId);
                return new DefaultPlayerColorDestinyCard(gamePlayer);
            }

            Logger.Warning("DestinyCardFactory.Create: card is not supported.");
            return ErrorDestinyCard.Instance;
        }
    }
}