using System;
using Client.Game.Field;
using Core.Game;
using Core.Game.Phases;
using VContainer.Unity;

namespace Client.Game
{
    public class GamePhaseController : IPhaseVisitor, IStartable, IDisposable
    {
        private readonly GameStateMachine _stateMachine;
        private readonly FieldObjectsCreator _fieldObjectsCreator;
        
        public GamePhaseController(GameStateMachine stateMachine, FieldObjectsCreator fieldObjectsCreator)
        {
            _stateMachine = stateMachine;
            _fieldObjectsCreator = fieldObjectsCreator;
            _stateMachine.OnPhaseChanged += PhaseChanged;
        }
        
        void IPhaseVisitor.Visit(GameInitializationPhase phase)
        {
            _fieldObjectsCreator.InitPlanets();
        }
        
        void IStartable.Start()
        {
            if (_stateMachine.CurrentPhase != null)
            {
                PhaseChanged(_stateMachine.CurrentPhase);
            }
        }

        void IDisposable.Dispose()
        {
            _stateMachine.OnPhaseChanged -= PhaseChanged;
        }

        private void PhaseChanged(IGamePhase phase)
        {
            phase.Accept(this);
        }
    }
}