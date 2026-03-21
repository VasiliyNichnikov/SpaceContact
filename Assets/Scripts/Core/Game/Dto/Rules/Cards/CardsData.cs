namespace Core.Game.Dto.Rules.Cards
{
    public class CardsData
    {
        public readonly int PlayerStartingNumberOfSpaceCards;

        public readonly DecksData Decks;
        
        public readonly DestinyCardsGenerationData DestinyCardsGeneration;

        public CardsData(
            DecksData decks, 
            int playerStartingNumberOfSpaceCards,
            DestinyCardsGenerationData destinyCardsGeneration)
        {
            PlayerStartingNumberOfSpaceCards = playerStartingNumberOfSpaceCards;
            Decks = decks;
            DestinyCardsGeneration = destinyCardsGeneration;
        }
    }
}