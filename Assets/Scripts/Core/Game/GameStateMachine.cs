using System;
using System.Threading.Tasks;
using Core.Game.Factory;
using Core.Game.Phases;

namespace Core.Game
{
    public class GameStateMachine : IGameStateMachineReadOnly, IDisposable
    {
        private readonly IPhaseFactory _phaseFactory;

        public GameStateMachine(IPhaseFactory phaseFactory)
        {
            _phaseFactory = phaseFactory;
        }
        
        public IGamePhase? CurrentPhase { get; private set; }
        
        public event Action? OnPhaseChanged;

        public Task TransitionTo<T>(IPhasePayload? payload) where T : IGamePhase => 
            TransitionTo(typeof(T), payload);

        public async Task TransitionTo(Type phaseType, IPhasePayload? payload)
        {
            CurrentPhase?.Exit();

            CurrentPhase = _phaseFactory.Create(phaseType, payload);
            await CurrentPhase.Enter();
            
            OnPhaseChanged?.Invoke();
        }

        public void Update()
        {
            CurrentPhase?.Update();
        }

        public void Dispose()
        {
            CurrentPhase?.Exit();
            CurrentPhase = null;
        }
    }
}