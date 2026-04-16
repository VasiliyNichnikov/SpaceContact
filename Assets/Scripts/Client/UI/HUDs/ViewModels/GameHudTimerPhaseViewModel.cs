using Core.Game;
using Core.Game.Phases;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudTimerPhaseViewModel : IGameHudTimerPhaseViewModel
    {
        private readonly ReactivityProperty<int> _remainingTimeInSeconds = new();
        
        private readonly IGameStateMachineReadOnly _stateMachine;
        private IGamePhaseWithContext? _phaseWithContext;
        
        public GameHudTimerPhaseViewModel(IGameStateMachineReadOnly stateMachine)
        {
            _stateMachine = stateMachine;
            _stateMachine.OnPhaseChanged += TryRefreshPhaseTimer;
            TryRefreshPhaseTimer();
        }

        public IReactivityProperty<int> RemainingTimeInSeconds => 
            _remainingTimeInSeconds;

        public void Update()
        {
            if (_phaseWithContext == null)
            {
                return;
            }

            _remainingTimeInSeconds.Value = _phaseWithContext.RemainingTime;
        }
        
        public void Dispose()
        {
            _stateMachine.OnPhaseChanged -= TryRefreshPhaseTimer;
        }

        private void TryRefreshPhaseTimer()
        {
            var phase = _stateMachine.CurrentPhase;

            if (phase is IGamePhaseWithContext phaseWithContext)
            {
                _phaseWithContext = phaseWithContext;
            }
        }
    }
}