using Core.Game.Phases;
using Network.Infrastructure;

namespace App.Game.Services
{
    public class PhaseRegistrationService
    {
        private const byte GameInitializationPhaseId = 100;
        
        private readonly PhaseRegistry _phaseRegistry;
        
        public PhaseRegistrationService(PhaseRegistry phaseRegistry)
        {
            _phaseRegistry = phaseRegistry;
        }
        
        public void ConfigureRegistry()
        {
            _phaseRegistry.RegisterPhase<GameInitializationPhase>(GameInitializationPhaseId);
        }
    }
}