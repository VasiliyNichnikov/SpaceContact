using System;
using Core.Factory;
using Core.Phases;

namespace Core
{
    public class GameStateMachine
    {
        private readonly IPhaseFactory _phaseFactory;

        public GameStateMachine(IPhaseFactory phaseFactory)
        {
            _phaseFactory = phaseFactory;
        }
        
        public IGamePhase? CurrentPhase { get; private set; }
        
        public event Action<IGamePhase>? OnPhaseChanged;

        public void TransitionTo<T>(IPhasePayload? payload) where T : IGamePhase => 
            TransitionTo(typeof(T), payload);

        public void TransitionTo(Type phaseType, IPhasePayload? payload)
        {
            CurrentPhase?.Exit();

            CurrentPhase = _phaseFactory.Create(phaseType, payload);
            CurrentPhase.Enter();
            
            OnPhaseChanged?.Invoke(CurrentPhase);
        }

        public void Update()
        {
            CurrentPhase?.Update();
        }
    }
}