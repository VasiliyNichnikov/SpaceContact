using Core.Game.Players;

namespace Core.Game
{
    public interface IGameFieldManager
    {
        IGamePlayer CurrentPlayer { get; }
        
        IGamePlayer OpponentPlayer { get; }

        void Init();
    }
}