#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Client.Configs.Game;

namespace Editor
{
    public sealed class CardsConfigValidation : IGameRulesValidation
    {
        public void Validate(GameRulesValidationContext context)
        {
            context.ValidateAssets<CardsConfig>(config => ValidateCards(config, context.MaximumPlayerCount));
        }

        private static void ValidateCards(CardsConfig config, int maximumPlayerCount)
        {
            var data = config.BuildData();

            if (data.PlayerStartingNumberOfSpaceCards < 0 || data.Decks.NumberOfConversationsSpaceCards < 0)
            {
                throw new InvalidOperationException("Starting hand size and conversation card count cannot be negative.");
            }

            long spaceCardCount = data.Decks.NumberOfConversationsSpaceCards;

            foreach (var damage in data.Decks.DamageSpaceCards)
            {
                if (damage.Count < 0 || damage.DamageCount < 0)
                {
                    throw new InvalidOperationException("Damage card count and damage cannot be negative.");
                }

                spaceCardCount += damage.Count;
            }

            var requiredCardCount = (long)maximumPlayerCount * data.PlayerStartingNumberOfSpaceCards;

            if (spaceCardCount > int.MaxValue || spaceCardCount < requiredCardCount)
            {
                throw new InvalidOperationException(
                    $"Space deck size {spaceCardCount} must cover {requiredCardCount} starting cards and fit in a list.");
            }

            var generation = data.DestinyCardsGeneration;

            if (generation.NumberOfJokers < 0)
            {
                throw new InvalidOperationException("Joker card count cannot be negative.");
            }

            for (var playerCount = GameRulesValidationContext.MinimumPlayerCount; playerCount <= maximumPlayerCount; playerCount++)
            {
                int colorCardCount;

                try
                {
                    colorCardCount = generation.GetNumberOfColorCards(playerCount);
                }
                catch (KeyNotFoundException)
                {
                    throw new InvalidOperationException($"Destiny card rules are missing for {playerCount} players.");
                }

                var destinyCardCount = (long)playerCount * colorCardCount + generation.NumberOfJokers;

                if (colorCardCount < 0 || destinyCardCount <= 0 || destinyCardCount > int.MaxValue)
                {
                    throw new InvalidOperationException($"Invalid Destiny card counts for {playerCount} players.");
                }
            }
        }
    }
}
#endif
