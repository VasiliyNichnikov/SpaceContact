using System.Threading.Tasks;
using Core.Game.Encounter;
using Logs;

namespace Core.Game.Phases
{
    public sealed class GameFirstMovePhase : BasePhase
    {
        private readonly IGameServerEncounterManager? _serverEncounterManager;

        public GameFirstMovePhase(IGameServerEncounterManager? serverEncounterManager)
        {
            _serverEncounterManager = serverEncounterManager;
        }
        
        public override GamePhaseType Type => 
            GamePhaseType.FirstMove;
        
        public override Task Enter()
        {
            Logger.Warning($"{nameof(GameFirstMovePhase)}.{nameof(Enter)}");
            _serverEncounterManager?.StartEncounter();
            return Task.CompletedTask;
        }

        public override void Accept(IPhaseVisitor visitor) => 
            visitor.Visit(this);
    }
}