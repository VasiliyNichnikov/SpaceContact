using System;
using System.Collections.Generic;
using Core.Game.Dto.Rules.Cards;
using Core.Game.Dto.Snapshots;

namespace Core.Game.Cards
{
    public sealed class CardsInitialStateFactory
    {
        private static readonly Random _random = new();
        private readonly CardsData _data;

        public CardsInitialStateFactory(CardsData data)
        {
            _data = data;
        }
        
        public CardsSnapshot Create(IReadOnlyCollection<GamePlayerSnapshot> players)
        {
            var spaceDeck = CreateSpaceDeck(_data);
            var state = new CardsSnapshot
            {
                SpaceDeck = spaceDeck,
                DestinyDeck = CreateDestinyCards(_data, players)
            };

            foreach (var player in players)
            {
                var cards = DrawStartingSpaceCards(spaceDeck, _data.PlayerStartingNumberOfSpaceCards);
                state.HandsByPlayerId.Add(player.PlayerId, new KnownPlayerHandSnapshot(cards));
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
            
            Shuffle(cards);

            return cards;
        }

        private static List<DestinyCardSnapshot> CreateDestinyCards(CardsData data,
            IReadOnlyCollection<GamePlayerSnapshot> players)
        {
            var cards = new List<DestinyCardSnapshot>();
            var generationData = data.DestinyCardsGeneration;

            CallMethodMultipleTimes(() =>
            {
                var jokerCard = DestinyCardSnapshot.JokerCard();
                cards.Add(jokerCard);
            }, generationData.NumberOfJokers);

            var playersCount = players.Count;
            var numberOfColorCards = generationData.GetNumberOfColorCards(playersCount);

            foreach (var player in players)
            {
                CallMethodMultipleTimes(() =>
                {
                    var damageStateCard = DestinyCardSnapshot.ColorCard(player.PlayerId);
                    cards.Add(damageStateCard);
                }, numberOfColorCards);
            }
            
            // TODO: Еще надо добавить специфичные карты

            return cards;
        }

        private static List<SpaceCardSnapshot> DrawStartingSpaceCards(List<SpaceCardSnapshot> deck, int numberOfSpaceCards)
        {
            var cards = new List<SpaceCardSnapshot>();

            for (var i = 0; i < numberOfSpaceCards; i++)
            {
                var card = deck[0];
                deck.RemoveAt(0);
                cards.Add(card);
            }
            
            return cards;
        }
        
        private static void CallMethodMultipleTimes(Action action, int count)
        {
            for (var i = 0; i < count; i++)
            {
                action.Invoke();
            }
        }
        
        private static void Shuffle<T>(IList<T> list)
        {
            var n = list.Count;
            
            while (n > 1)
            {
                n--;
                var k = _random.Next(n + 1);
            
                // Меняем элементы местами (кортежный синтаксис C#)
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
