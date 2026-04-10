using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;
using Logs;

namespace Core.Game.Phases.Client
{
    public sealed class GameClientDestinyPhaseResolver : IGameClientDestinyPhaseResolver
    {
        private readonly DestinyCardFactory _destinyCardFactory;
        private readonly IGamePlayerHandServerInteraction _serverInteraction;
        
        public GameClientDestinyPhaseResolver(
            DestinyCardFactory destinyCardFactory,
            IGamePlayerHandServerInteraction serverInteraction)
        {
            _destinyCardFactory = destinyCardFactory;
            _serverInteraction = serverInteraction;
        }
        
        public event Action? Changed;
        
        public IDestinyCard? Card { get; private set; }
        
        public bool IsWaitingServer { get; private set; }

        public async Task SkipDestinyAsync(CancellationToken ct)
        {
            if (IsWaitingServer)
            {
                Logger.Error($"{nameof(GameClientDestinyPhaseResolver)}.{nameof(SkipDestinyAsync)}: waiting for a response from the server.");
                return;
            }

            IsWaitingServer = true;
            var isCompleted = await _serverInteraction.TrySkipCardAsync(ct);
            IsWaitingServer = false;

            if (ct.IsCancellationRequested)
            {
                return;
            }
            
            if (!isCompleted)
            {
                Logger.Error($"{nameof(GameClientDestinyPhaseResolver)}.{nameof(SkipDestinyAsync)}: cannot skip destiny card.");
            }
            else
            {
                Logger.Log($"{nameof(GameClientDestinyPhaseResolver)}.{nameof(SkipDestinyAsync)}: card changed.");
            }
        }

        public void UpdateState(DestinyCardData state)
        {
            Card = _destinyCardFactory.Create(state);
            Changed?.Invoke();
        }
    }
}