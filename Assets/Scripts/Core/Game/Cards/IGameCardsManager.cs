using Core.Game.Dto.States.Cards;

namespace Core.Game.Cards
{
    public interface IGameCardsManager
    {
        void Init();
        
        PlayerHandStateData CreatePlayerHand();
        
        DestinyCardStateData OpenNextDestinyCard();
    }
}