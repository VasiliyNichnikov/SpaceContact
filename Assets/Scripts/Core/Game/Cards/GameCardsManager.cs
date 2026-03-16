using System;
using System.Collections.Generic;
using Core.Game.Dto.Rules.Cards;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Cards
{
    public class GameCardsManager : IGameCardsManager
    {
        private static readonly Random _random = new();

        private readonly CardsData _data;
        private readonly Queue<SpaceCardStateData> _collectedSpaceCards;
        
        public GameCardsManager(CardsData data)
        {
            _data = data;
            _collectedSpaceCards = CreateCollectedSpaceCards(_data);
        }

        PlayerHandStateData IGameCardsManager.CreatePlayerHand()
        {
            var playerHand = new List<SpaceCardStateData>(_data.PlayerStartingNumberOfSpaceCards);
                
            for (var i = 0; i < _data.PlayerStartingNumberOfSpaceCards; i++)
            {
                var cardForPlayer = _collectedSpaceCards.Dequeue();
                playerHand.Add(cardForPlayer);
            }

            var handState = new PlayerHandStateData
            {
                SpaceCardsOnYourHand = playerHand,
                NumberOfCards =  playerHand.Count
            };
            
            return handState;
        }

        private static Queue<SpaceCardStateData> CreateCollectedSpaceCards(CardsData data)
        {
            var cards = new List<SpaceCardStateData>();
            var decks = data.Decks;

            for (var i = 0; i < decks.NumberOfConversationsSpaceCards; i++)
            {
                var conversationStateCard = SpaceCardStateData.ConversationStateCard();
                cards.Add(conversationStateCard);
            }

            foreach (var damageData in decks.DamageSpaceCards)
            {
                CallMethodMultipleTimes(() =>
                {
                    var damageStateCard = SpaceCardStateData.DamageStateCard(damageData.DamageCount);
                    cards.Add(damageStateCard);
                }, damageData.Count);
            }

            foreach (var artifactData in decks.ArtifactsSpaceCards)
            {
                CallMethodMultipleTimes(() =>
                {
                    var damageStateCard = SpaceCardStateData.ArtifactStateCard(artifactData.ArtifactId);
                    cards.Add(damageStateCard);
                }, artifactData.Count);
            }
            
            Shuffle(cards);

            return new Queue<SpaceCardStateData>(cards);
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