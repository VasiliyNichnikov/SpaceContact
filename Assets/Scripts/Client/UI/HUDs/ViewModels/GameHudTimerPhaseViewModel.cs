using Core.Game;
using Core.Game.Phases;
using Core.Game.Phases.Client;
using GeneralUtils;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudTimerPhaseViewModel : IGameHudTimerPhaseViewModel
    {
        private readonly ReactivityProperty<int> _remainingTimeInSeconds = new();
        private readonly ReactivityProperty<bool> _isReadyToNextPhase = new();
        
        private readonly IGameStateMachineReadOnly _stateMachine;
        private readonly GameClientPlayerReadinessController _readinessController;
        
        private IGamePhaseWithContext? _phaseWithContext;
        
        public GameHudTimerPhaseViewModel(
            IGameStateMachineReadOnly stateMachine,
            GameClientPlayerReadinessController readinessController)
        {
            _stateMachine = stateMachine;
            _readinessController = readinessController;
            _stateMachine.OnPhaseChanged += TryRefreshPhaseTimer;
            _readinessController.Changed += ReadinessChanged;
            TryRefreshPhaseTimer();
        }

        public IReactivityProperty<int> RemainingTimeInSeconds => 
            _remainingTimeInSeconds;

        public IReactivityProperty<bool> IsReadyToNextPhase => 
            _isReadyToNextPhase;

        public void OnReadyButtonClickHandler() => 
            _readinessController
                .SwitchReadinessAsync()
                .FireAndForget();

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
            _readinessController.Changed -= ReadinessChanged;
        }

        private void TryRefreshPhaseTimer()
        {
            var phase = _stateMachine.CurrentPhase;

            if (phase is IGamePhaseWithContext phaseWithContext)
            {
                _phaseWithContext = phaseWithContext;
            }
        }

        private void ReadinessChanged()
        {
            _isReadyToNextPhase.Value = _readinessController.IsReadyToNextPhase;
        }
    }
}