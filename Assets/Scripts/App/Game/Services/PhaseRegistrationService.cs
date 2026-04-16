using Core.Game.Phases;
using Network.Infrastructure;

namespace App.Game.Services
{
    public sealed class PhaseRegistrationService
    {
        private readonly PhaseRegistry _phaseRegistry;
        
        public PhaseRegistrationService(PhaseRegistry phaseRegistry) => 
            _phaseRegistry = phaseRegistry;
        
        public void ConfigureRegistry()
        {
            _phaseRegistry.RegisterPhase<GameInitializationPhase>(PhaseIds.GameInitializationPhaseId);
            _phaseRegistry.RegisterPhase<GameFirstMovePhase>(PhaseIds.GameFirstMovePhaseId);
            _phaseRegistry.RegisterPhase<GameRegroupPhase>(PhaseIds.GameRegroupingPhaseId);
            _phaseRegistry.RegisterPhase<GameDestinyPhase>(PhaseIds.GameDestinyPhaseId);
        }
    }
}