using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game.Players;
using Logs;

namespace Core.Game.Phases.Client
{
    public sealed class GameClientPlayerReadinessController : IGameClientPlayerReadinessEvents, IDisposable
    {
        private readonly IGamePhaseServerInteraction _serverInteraction;
        private readonly GamePlayersRegistry _playersRegistry;
        private readonly CancellationTokenSource _cts = new();
        private bool _isWaitingServer;
        private bool _isReadyToNextPhase;

        public GameClientPlayerReadinessController(IGamePhaseServerInteraction serverInteraction, GamePlayersRegistry playersRegistry)
        {
            _serverInteraction = serverInteraction;
            _playersRegistry = playersRegistry;
        }

        public bool IsReadyToNextPhase
        {
            get => _isReadyToNextPhase;
            
            private set
            {
                if (_isReadyToNextPhase == value)
                {
                    return;
                }

                _isReadyToNextPhase = value;
                Changed?.Invoke();
            }
        }

        public event Action? Changed;
        
        void IGameClientPlayerReadinessEvents.SetReady()
        {
            var player = _playersRegistry.GetOwnerWithError();
            player!.IsReadyToNextPhase = true;
            IsReadyToNextPhase = true;
        }

        void IGameClientPlayerReadinessEvents.SetNotReady()
        {
            var player = _playersRegistry.GetOwnerWithError();
            player!.IsReadyToNextPhase = false;
            IsReadyToNextPhase = false;
        }

        public Task SwitchReadinessAsync()
        {
            return _isWaitingServer 
                ? Task.CompletedTask 
                : SwitchReadinessInternalAsync();
        }

        public void SetAllNotReady()
        {
            foreach (var player in _playersRegistry.Players)
            {
                player.IsReadyToNextPhase = false;
            }
            
            IsReadyToNextPhase = false;
        }
        
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        private async Task SwitchReadinessInternalAsync()
        {
            bool isCompleted;
            _isWaitingServer = true;
            
            if (IsReadyToNextPhase)
            {
                isCompleted = await _serverInteraction.NotReadyToNextPhaseAsync(_cts.Token);
            }
            else
            {
                isCompleted = await _serverInteraction.ReadyToNextPhaseAsync(_cts.Token);
            }

            _isWaitingServer = false;

            if (_cts.Token.IsCancellationRequested)
            {
                return;
            }

            if (!isCompleted)
            {
                Logger.Error($"{nameof(GameClientPlayerReadinessController)}.{nameof(SwitchReadinessInternalAsync)}: the operation could not be performed.");
            }
        }
    }
}