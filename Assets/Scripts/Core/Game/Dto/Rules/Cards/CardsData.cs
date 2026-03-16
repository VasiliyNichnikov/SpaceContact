namespace Core.Game.Dto.Rules.Cards
{
    public class CardsData
    {
        public readonly int PlayerStartingNumberOfSpaceCards;

        public readonly DecksData Decks;

        public CardsData(DecksData decks, int playerStartingNumberOfSpaceCards)
        {
            PlayerStartingNumberOfSpaceCards = playerStartingNumberOfSpaceCards;
            Decks = decks;
        }
    }
}