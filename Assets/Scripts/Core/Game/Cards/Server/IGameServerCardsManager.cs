using Core.Game.Dto.States.Cards;

namespace Core.Game.Cards
{
    public interface IGameServerCardsManager
    {
        void Init();
        
        PlayerHandStateData CreatePlayerHand();
        
        DestinyCardData OpenNextDestinyCard();
    }
}