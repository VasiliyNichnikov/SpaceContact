using System.Collections.Generic;

namespace Core.Game
{
    public interface IGameFieldManager
    {
        bool IsReady { get; }

        GamePlayerData CurrentPlayer { get; }
        
        IReadOnlyCollection<GamePlayerData> Opponents { get; }
    }
}
