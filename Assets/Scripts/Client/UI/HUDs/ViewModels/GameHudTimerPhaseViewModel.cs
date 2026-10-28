using System;
using Core.Game;
using Core.Game.Actions;
using Core.Helpers;
using Reactivity;
using VContainer;
using VContainer.Unity;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudTimerPhaseViewModel : IGameHudTimerPhaseViewModel, IInitializable, ITickable, IDisposable
    {
        private readonly ReactivityProperty<int> _remainingTimeInSeconds = new();
        private readonly ReactivityProperty<bool> _isReadyToNextPhase = new();
        private readonly ReactivityProperty<bool> _canToggleReady = new();

        [Inject] private readonly ClientGameContext _gameContext = null!;
        [Inject] private readonly GameActionClient _actionClient = null!;
        [Inject] private readonly PendingGameActions _pendingActions = null!;

        public IReactivityProperty<int> RemainingTimeInSeconds => _remainingTimeInSeconds;

        public IReactivityProperty<bool> IsReadyToNextPhase => _isReadyToNextPhase;

        public IReactivityProperty<bool> CanToggleReady => _canToggleReady;

        public void OnReadyButtonClickHandler()
        {
            var action = new TogglePlayerReadyGameAction(_gameContext.Phase.Revision);
            
            if (!_actionClient.ValidateAction(action))
            {
                return;
            }

            _actionClient.Send(action);
        }

        public void Dispose()
        {
            _gameContext.StateChanged -= Refresh;
            _pendingActions.Changed -= Refresh;
        }

        void IInitializable.Initialize()
        {
            _gameContext.StateChanged += Refresh;
            _pendingActions.Changed += Refresh;
            Refresh();
        }

        void ITickable.Tick()
        {
            Refresh();
        }

        private void Refresh()
        {
            _canToggleReady.Value = _actionClient.ValidateAction(new TogglePlayerReadyGameAction(_gameContext.Phase.Revision));
            _isReadyToNextPhase.Value = _gameContext.LocalPlayerState?.IsReady ?? false;
            var deadline = GetDeadline();
            var remaining = deadline.HasValue
                ? Math.Max(0, Math.Ceiling(deadline.Value - _gameContext.Time.ServerTimeInSeconds))
                : 0;
            _remainingTimeInSeconds.Value = (int)Math.Min(int.MaxValue, remaining);
        }

        private double? GetDeadline()
        {
            if (_gameContext.LocalPlayerState is null || !TransitionPhaseHelper.IsTimedPhase(_gameContext.Phase.Type))
            {
                return null;
            }

            var phase = _gameContext.Phase;
            if (!phase.EndTimeInSeconds.HasValue)
            {
                return null;
            }

            return phase.ReadyTransitionTimeInSeconds.HasValue
                ? Math.Min(phase.EndTimeInSeconds.Value, phase.ReadyTransitionTimeInSeconds.Value)
                : phase.EndTimeInSeconds;
        }
    }
}
