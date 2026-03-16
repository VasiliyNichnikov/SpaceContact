using System;
using System.Collections.Generic;
using Core.Game.Cards;

namespace Core.Game.Hands
{
    public class EmptyGamePlayerHandController : IGamePlayerHandController
    {
        private static EmptyGamePlayerHandController? _instance;
        
        private EmptyGamePlayerHandController()
        {
            // nothing
        }

        public static IGamePlayerHandController Instance => 
            _instance ??= new EmptyGamePlayerHandController();

        public event Action? OnInitialized;
        
        public event Action<ISpaceCard>? OnCardAdded;
        
        public event Action<ISpaceCard>? OnCardRemoved;

        public int SpaceCardsCount => 0;
        
        public IReadOnlyCollection<ISpaceCard> SpaceCards => 
            Array.Empty<ISpaceCard>();
        
        public void AddSpaceCard(ISpaceCard spaceCard)
        {
            // nothing
        }

        public void RemoveSpaceCard(ISpaceCard spaceCard)
        {
            // nothing
        }
    }
}