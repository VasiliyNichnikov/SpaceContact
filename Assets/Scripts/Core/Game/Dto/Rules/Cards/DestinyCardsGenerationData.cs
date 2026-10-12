using System.Collections.Generic;

namespace Core.Game.Dto.Rules.Cards
{
    public readonly struct DestinyCardsGenerationData
    {
        private readonly IReadOnlyDictionary<int, int> _numberOfColorCardsByPlayerCount;

        public DestinyCardsGenerationData(
            IReadOnlyDictionary<int, int> numberOfColorCardsByPlayerCount,
            int numberOfJokers)
        {
            _numberOfColorCardsByPlayerCount = numberOfColorCardsByPlayerCount;
            NumberOfJokers = numberOfJokers;
        }
        
        public int NumberOfJokers { get; }

        public int GetNumberOfColorCards(int playerCount)
        {
            return _numberOfColorCardsByPlayerCount[playerCount];
        }
    }
}
