using VContainer.Unity;

namespace App.Game.Services
{
    public class GameStartupService : IStartable
    {
        private readonly PhaseRegistrationService _phaseRegistrationService;

        public GameStartupService(PhaseRegistrationService phaseRegistrationService)
        {
            _phaseRegistrationService = phaseRegistrationService;
        }
        
        public void Start()
        {
            _phaseRegistrationService.ConfigureRegistry();
        }
    }
}