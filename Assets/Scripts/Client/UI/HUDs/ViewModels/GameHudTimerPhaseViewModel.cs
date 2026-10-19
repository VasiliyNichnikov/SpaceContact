using System;
using Core.Game;
using Core.Game.Actions;
using Core.Helpers;
using Reactivity;
using VContainer;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudTimerPhaseViewModel : IGameHudTimerPhaseViewModel
    {
        private readonly ReactivityProperty<int> _remainingTimeInSeconds = new();
        private readonly ReactivityProperty<bool> _isReadyToNextPhase = new();

        [Inject] private readonly GameContext _gameContext = null!;
        [Inject] private readonly GameActionClient _actionClient = null!;
        [Inject] private readonly PendingGameActions _pendingActions = null!;

        public IReactivityProperty<int> RemainingTimeInSeconds => _remainingTimeInSeconds;

        public IReactivityProperty<bool> IsReadyToNextPhase => _isReadyToNextPhase;

        public void Init()
        {
            _gameContext.StateChanged += Refresh;
            Refresh();
        }

        public void OnReadyButtonClickHandler()
        {
            if (_gameContext.LocalPlayerState is null ||
                _pendingActions.Contains<TogglePlayerReadyGameAction>())
            {
                return;
            }

            var deadline = GetDeadline();
            if (!deadline.HasValue || _gameContext.Time.ServerTimeInSeconds >= deadline.Value)
            {
                return;
            }

            _actionClient.Send(new TogglePlayerReadyGameAction());
        }

        public void Update()
        {
            Refresh();
        }

        public void Dispose()
        {
            _gameContext.StateChanged -= Refresh;
        }

        private void Refresh()
        {
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
