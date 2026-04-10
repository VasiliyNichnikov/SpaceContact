using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game.Cards;
using Core.Game.Dto.States.Cards;

namespace Core.Game.Phases.Client
{
    public interface IGameClientDestinyPhaseResolver
    {
        event Action? Changed;
        
        IDestinyCard? Card { get; }
        
        bool IsWaitingServer { get; }
        
        Task SkipDestinyAsync(CancellationToken ct = default);
        
        void UpdateState(DestinyCardData state);
    }
}