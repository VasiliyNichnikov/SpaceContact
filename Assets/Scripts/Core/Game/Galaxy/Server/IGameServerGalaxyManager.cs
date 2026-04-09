using Core.Game.Dto.States;

namespace Core.Game.Galaxy.Server
{
    public interface IGameServerGalaxyManager
    {
        void Init();
        
        GalaxyStateData ToState();
    }
}