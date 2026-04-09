using System.Threading.Tasks;

namespace Core.Game.Phases
{
    public interface IGamePhase
    {
        GamePhaseType Type { get; }
        
        Task Enter();

        void Exit();
        
        void Update();
        
        void Accept(IPhaseVisitor visitor);
    }
}