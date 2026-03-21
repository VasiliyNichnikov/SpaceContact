using System;
using System.Collections.Generic;
using Core.Game.Cards;

namespace Core.Game.Hands
{
    /// <summary>
    /// Управляет рукой игрока
    /// </summary>
    public interface IGamePlayerHandController
    {
        event Action? OnRefreshed;
        
        event Action<ISpaceCard>? OnCardAdded;
        
        event Action<ISpaceCard>? OnCardRemoved;
        
        int SpaceCardsCount { get; }
        
        IReadOnlyCollection<ISpaceCard> SpaceCards { get; }
        
        void AddSpaceCard(ISpaceCard spaceCard);
        
        void RemoveSpaceCard(ISpaceCard spaceCard);
    }
}