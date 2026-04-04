using System;
using Core.Game.Players;

namespace Client.Game.Field
{
    public interface IGameFieldViewManager
    {
        event Action? OnViewedOpponentChanged;
        
        IGamePlayer? ViewedOpponentPlayer { get; }
        
        void Init();
        
        bool CanMoveToLeftOpponent();
        
        bool CanMoveToRightOpponent();
        
        void MoveToLeftOpponent();
        
        void MoveToRightOpponent();
    }
}