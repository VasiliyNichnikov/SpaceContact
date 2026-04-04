namespace Client.Game.Field
{
    public interface IGameFieldViewManager
    {
        void Init();
        
        bool CanMoveToLeftOpponent();
        
        bool CanMoveToRightOpponent();
        
        void MoveToLeftOpponent();
        
        void MoveToRightOpponent();
    }
}