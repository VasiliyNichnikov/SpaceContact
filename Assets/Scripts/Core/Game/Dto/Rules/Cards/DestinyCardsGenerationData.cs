using System.Collections.Generic;
using Logs;

namespace Core.Game.Dto.Rules.Cards
{
    public readonly struct DestinyCardsGenerationData
    {
        private const int DefaultNumberOfColorCards = 3;
        
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
            if (_numberOfColorCardsByPlayerCount.TryGetValue(playerCount, out var numberOfColorCards))
            {
                return numberOfColorCards;
            }
            
            Logger.Error($"DestinyCardsGenerationData.GetNumberOfColorCards: no data found for {playerCount} players.");

            return DefaultNumberOfColorCards;
        }
    }
}