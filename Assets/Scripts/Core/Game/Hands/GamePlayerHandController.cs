using System;
using System.Collections.Generic;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Logs;

namespace Core.Game.Hands
{
    public class GamePlayerHandController : IGamePlayerHandController
    {
        private readonly SpaceCardFactory _factory;
        private readonly List<ISpaceCard> _cards = new();
        
        public GamePlayerHandController(SpaceCardFactory factory) => 
            _factory = factory;

        public event Action? OnRefreshed;
        
        public event Action<ISpaceCard>? OnCardAdded;
        
        public event Action<ISpaceCard>? OnCardRemoved;
        
        public int SpaceCardsCount => 
            _cards.Count;

        public IReadOnlyCollection<ISpaceCard> SpaceCards => 
            _cards;

        public void UpdateState(PlayerHandStateData state)
        {
            _cards.Clear();

            if (state.SpaceCardsOnYourHand == null)
            {
                Logger.Error("GamePlayerHandController.UpdateState: spaceCardsOnYourHand is null.");
                return;
            }

            foreach (var spaceCardStateData in state.SpaceCardsOnYourHand)
            {
                var createdSpaceCard = _factory.Create(spaceCardStateData);
                _cards.Add(createdSpaceCard);
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
            _cards.Remove(spaceCard);
            OnCardRemoved?.Invoke(spaceCard);
        }
    }
}