using System;
using System.Collections.Generic;
using Core.Game.Dto.Rules.Cards;
using Core.Game.Dto.States.Cards;
using Core.Game.Players;
using Logs;

namespace Core.Game.Cards
{
    public class GameCardsManager : IGameCardsManager
    {
        private static readonly Random _random = new();

        private readonly CardsData _data;
        private readonly GamePlayersRegistry _playersRegistry;
        
        private readonly Queue<SpaceCardStateData> _collectedSpaceCards = new();
        private readonly Queue<DestinyCardStateData> _collectedDestinyCards = new();

        private List<DestinyCardStateData>? _discardDestinyCards; 
        
        private DestinyCardStateData? _currentDestinyCard;
        
        public GameCardsManager(CardsData data, GamePlayersRegistry playersRegistry)
        {
            _data = data;
            _playersRegistry = playersRegistry;
        }

        void IGameCardsManager.Init()
        {
            CollectSpaceCards(_collectedSpaceCards, _data);
            CollectDestinyCards(_collectedDestinyCards, _data, _playersRegistry.Players);
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

        DestinyCardStateData IGameCardsManager.OpenNextDestinyCard()
        {
            if (_currentDestinyCard != null)
            {
                _discardDestinyCards ??= new List<DestinyCardStateData>();
                _discardDestinyCards.Add(_currentDestinyCard.Value);
            }

            if (_collectedDestinyCards.Count > 0)
            {
                _currentDestinyCard = _collectedDestinyCards.Dequeue();

                return _currentDestinyCard.Value;
            }
            
            RebuildDeck(ref _discardDestinyCards, _collectedDestinyCards);
            _currentDestinyCard = _collectedDestinyCards.Dequeue();
            
            return _currentDestinyCard.Value;
        }

        private static void CollectSpaceCards(Queue<SpaceCardStateData> deck, CardsData data)
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

            foreach (var card in cards)
            {
                deck.Enqueue(card);
            }
        }

        private static void CollectDestinyCards(Queue<DestinyCardStateData> deck, CardsData data, IReadOnlyCollection<IGamePlayer> players)
        {
            var cards = new List<DestinyCardStateData>();
            var generationData = data.DestinyCardsGeneration;

            CallMethodMultipleTimes(() =>
            {
                var jokerCard = DestinyCardStateData.JokerCard();
                cards.Add(jokerCard);
            }, generationData.NumberOfJokers);

            var playersCount = players.Count;
            var numberOfColorCards = generationData.GetNumberOfColorCards(playersCount);

            foreach (var player in players)
            {
                CallMethodMultipleTimes(() =>
                {
                    var damageStateCard = DestinyCardStateData.ColorCard(player.PlayerId);
                    cards.Add(damageStateCard);
                }, numberOfColorCards);
            }
            
            // Еще надо добавить специфичные карты
            
            Shuffle(cards);

            foreach (var card in cards)
            {
                deck.Enqueue(card);
            }
        }
        
        private static void CallMethodMultipleTimes(Action action, int count)
        {
            for (var i = 0; i < count; i++)
            {
                action.Invoke();
            }
        }
        
        private static void RebuildDeck<TDeck>(ref List<TDeck>? discardDeck, Queue<TDeck> deck)
        {
            if (discardDeck == null || discardDeck.Count == 0)
            {
                Logger.Error("GameCardsManager.RebuildDeck: discardDeck is null or empty.");
                
                return;
            }

            if (deck.Count != 0)
            {
                Logger.Error("GameCardsManager.RebuildDeck: deck is not empty.");
                
                return;
            }
            
            Shuffle(discardDeck);
            deck.Clear();

            foreach (var card in discardDeck)
            {
                deck.Enqueue(card);
            }

            discardDeck = null;
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