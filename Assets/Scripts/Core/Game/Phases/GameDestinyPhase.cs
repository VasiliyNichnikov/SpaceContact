using System.Threading.Tasks;
using Core.Game.Dto.Payload;
using Core.Game.Encounter;
using Core.Game.Phases.Server;
using Logs;

namespace Core.Game.Phases
{
    public sealed class GameDestinyPhase : BasePhaseWithContext<GamePhaseDestinyPayload>
    {
        private readonly IGameClientEncounterManager _clientEncounterManager;
        private readonly IGameServerDestinyPhaseResolver? _serverDestinyPhaseResolver;
        
        public GameDestinyPhase(
            IGameClientEncounterManager clientEncounterManager,
            IGameServerDestinyPhaseResolver? serverDestinyPhaseResolver, 
            GameStateMachine stateMachine) : base(stateMachine)
        {
            _clientEncounterManager = clientEncounterManager;
            _serverDestinyPhaseResolver = serverDestinyPhaseResolver;
        }

        public override GamePhaseType Type => 
            GamePhaseType.Destiny;

        public override Task Enter()
        {
            if (Context == null)
            {
                Logger.Error($"{nameof(GameDestinyPhase)}.{nameof(Enter)}: context is null.");
                
                return Task.CompletedTask;
            }
            
            Logger.Warning("GameDestinyPhase.Enter");
            _clientEncounterManager.UpdateState(Context.EncounterState);
            _serverDestinyPhaseResolver?.ChooseDestiny();
            
            return Task.CompletedTask;
        }

        public override void Accept(IPhaseVisitor visitor) => 
            visitor.Visit(this);
    }
}