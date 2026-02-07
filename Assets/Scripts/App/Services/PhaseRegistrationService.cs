using Core.Phases;
using Network.Infrastructure;

namespace App.Services
{
    public class PhaseRegistrationService
    {
        private const byte LobbyPhaseId = 100;
        private const byte RegroupPhaseId = 101;
        
        private readonly PhaseRegistry _phaseRegistry;
        
        public PhaseRegistrationService(PhaseRegistry phaseRegistry)
        {
            _phaseRegistry = phaseRegistry;
        }
        
        public void ConfigureRegistry()
        {
            _phaseRegistry.RegisterPhase<LobbyPhase>(LobbyPhaseId);
            _phaseRegistry.RegisterPhase<RegroupPhase>(RegroupPhaseId);
        }
    }
}