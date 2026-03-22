using Core.Game.Dto.States.Cards;

namespace Core.Game.Cards
{
    public interface IGameCardsManager
    {
        PlayerHandStateData CreatePlayerHand();
        
        DestinyCardStateData OpenNextDestinyCard();
    }
}