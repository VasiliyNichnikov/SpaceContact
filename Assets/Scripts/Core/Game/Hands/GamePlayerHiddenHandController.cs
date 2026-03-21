using System;
using System.Collections.Generic;
using Core.Game.Cards;

namespace Core.Game.Hands
{
    public class GamePlayerHiddenHandController : IGamePlayerHandController
    {
        private readonly List<ISpaceCard> _cards = new();
        
        public event Action? OnRefreshed;
        
        public event Action<ISpaceCard>? OnCardAdded;
        
        public event Action<ISpaceCard>? OnCardRemoved;
        
        public int SpaceCardsCount => 
            _cards.Count;
        
        public IReadOnlyCollection<ISpaceCard> SpaceCards => 
            _cards;

        public void UpdateState(int numberOfCards)
        {
            _cards.Clear();
            
            for (var i = 0; i < numberOfCards; i++)
            {
                _cards.Add(EmptySpaceCard.Instance);
            }
            
            OnRefreshed?.Invoke();
        }
        
        public void AddSpaceCard(ISpaceCard spaceCard)
        {
            _cards.Add(spaceCard);
            OnCardAdded?.Invoke(spaceCard);
        }

        public void RemoveSpaceCard(ISpaceCard spaceCard)
        {
            _cards.Add(spaceCard);
            OnCardRemoved?.Invoke(spaceCard);
        }
    }
}