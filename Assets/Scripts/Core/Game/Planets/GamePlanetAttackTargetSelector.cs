using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game.Encounter;
using Logs;

namespace Core.Game.Planets
{
    public sealed class GamePlanetAttackTargetSelector : IDisposable
    {
        private readonly IGameEncounterServerInteraction _serverInteraction;
        private readonly CancellationTokenSource _cts = new();
        
        public GamePlanetAttackTargetSelector(IGameEncounterServerInteraction serverInteraction)
        {
            _serverInteraction = serverInteraction;
        }
        
        public bool IsWaitingServer { get; private set; }
        
        public async Task SelectTargetAsync(int planetId)
        {
            if (IsWaitingServer)
            {
                Logger.Error($"{nameof(GamePlanetAttackTargetSelector)}.{nameof(SelectTargetAsync)}: the request has already been sent.");
                
                return;
            }
            
            IsWaitingServer = true;
            var isPlanetSelected = await _serverInteraction.ChoosePlanetToAttackAsync(planetId, _cts.Token);
            IsWaitingServer = false;

            if (_cts.IsCancellationRequested)
            {
                return;
            }
            
            if (!isPlanetSelected)
            {
                Logger.Error($"{nameof(GamePlanetAttackTargetSelector)}.{nameof(SelectTargetAsync)}: failed to select a planet to attack.");
            }
        }
        
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}