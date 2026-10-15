using System.Collections.Generic;
using System.Linq;
using System;
using Core.Game.Dto.Rules.Cards;
using Core.Game.Dto.Snapshots;

namespace Core.Game.Cards
{
    public sealed class CardsInitialStateFactory
    {
        private readonly CardsData _data;

        public CardsInitialStateFactory(CardsData data)
        {
            _data = data;
        }

        public CardsSnapshot Create(IReadOnlyCollection<GamePlayerSnapshot> players, int seed)
        {
            var numberOfColorCards = _data.DestinyCardsGeneration.GetNumberOfColorCards(players.Count);
            var sortedPlayers = players.OrderBy(player => player.Order).ToList();
            var random = new Random(seed);
            var spaceDeck = CreateSpaceDeck(_data);
            var requiredCardCount = (long)players.Count * _data.PlayerStartingNumberOfSpaceCards;

            if (spaceDeck.Count < requiredCardCount)
            {
                throw new InvalidOperationException(
                    $"Space deck has {spaceDeck.Count} cards; dealing requires {requiredCardCount}.");
            }

            var destinyDeck = CreateDestinyCards(_data, sortedPlayers, numberOfColorCards);
            Shuffle(spaceDeck, random);
            Shuffle(destinyDeck, random);

            var state = new CardsSnapshot
            {
                SpaceDeck = spaceDeck,
                DestinyDeck = destinyDeck
            };

            foreach (var player in sortedPlayers)
            {
                var cards = DrawStartingSpaceCards(spaceDeck, _data.PlayerStartingNumberOfSpaceCards);
                state.HandsByPlayerId.Add(player.PlayerId, new PlayerHandSnapshot(cards));
            }

            return state;
        }

        private static List<SpaceCardSnapshot> CreateSpaceDeck(CardsData data)
        {
            var cards = new List<SpaceCardSnapshot>();
            var decks = data.Decks;

            for (var i = 0; i < decks.NumberOfConversationsSpaceCards; i++)
            {
                var conversationStateCard = SpaceCardSnapshot.ConversationStateCard();
                cards.Add(conversationStateCard);
            }

            foreach (var damageData in decks.DamageSpaceCards)
            {
                CallMethodMultipleTimes(() =>
                {
                    var damageStateCard = SpaceCardSnapshot.DamageStateCard(damageData.DamageCount);
                    cards.Add(damageStateCard);
                }, damageData.Count);
            }

            // TODO: добавить артефакты
            // foreach (var artifactData in decks.ArtifactsSpaceCards)
            // {
            //     CallMethodMultipleTimes(() =>
            //     {
            //         var damageStateCard = SpaceCardStateData.ArtifactStateCard(artifactData.ArtifactId);
            //         cards.Add(damageStateCard);
            //     }, artifactData.Count);
            // }

            return cards;
        }

        private static List<DestinyCardSnapshot> CreateDestinyCards(CardsData data,
            IReadOnlyCollection<GamePlayerSnapshot> players, int numberOfColorCards)
        {
            var cards = new List<DestinyCardSnapshot>();
            var generationData = data.DestinyCardsGeneration;

            CallMethodMultipleTimes(() =>
            {
                var jokerCard = DestinyCardSnapshot.JokerCard();
                cards.Add(jokerCard);
            }, generationData.NumberOfJokers);

            foreach (var player in players)
            {
                CallMethodMultipleTimes(() =>
                {
                    var colorCard = DestinyCardSnapshot.ColorCard(player.PlayerId);
                    cards.Add(colorCard);
                }, numberOfColorCards);
            }

            // TODO: Еще надо добавить специфичные карты

            return cards;
        }

        private static List<SpaceCardSnapshot> DrawStartingSpaceCards(List<SpaceCardSnapshot> deck, int numberOfSpaceCards)
        {
            var cards = deck.GetRange(0, numberOfSpaceCards);
            deck.RemoveRange(0, numberOfSpaceCards);

            return cards;
        }

        private static void CallMethodMultipleTimes(Action action, int count)
        {
            for (var i = 0; i < count; i++)
            {
                action.Invoke();
            }
        }

        private static void Shuffle<T>(IList<T> list, Random random)
        {
            var n = list.Count;

            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);

                // Меняем элементы местами (кортежный синтаксис C#)
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
