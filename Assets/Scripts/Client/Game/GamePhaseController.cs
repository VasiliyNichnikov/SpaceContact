using System;
using Client.Game.Field;
using Client.UI.Loaders;
using Core.Game;
using Core.Game.Phases;
using VContainer.Unity;

namespace Client.Game
{
    public class GamePhaseController : IPhaseVisitor, IStartable, IDisposable
    {
        private readonly IGameStateMachineReadOnly _stateMachine;
        private readonly FieldObjectsCreator _fieldObjectsCreator;
        private readonly GameUILoader _gameUILoader;
        
        public GamePhaseController(
            IGameStateMachineReadOnly stateMachine, 
            FieldObjectsCreator fieldObjectsCreator,
            GameUILoader gameUILoader)
        {
            _stateMachine = stateMachine;
            _fieldObjectsCreator = fieldObjectsCreator;
            _gameUILoader = gameUILoader;
            _stateMachine.OnPhaseChanged += PhaseChanged;
        }
        
        void IPhaseVisitor.Visit(GameInitializationPhase phase)
        {
            _fieldObjectsCreator.InitPlanets();
            _gameUILoader.Load();
        }

        public void Visit(GameDestinyPhase phase)
        {
            // nothing
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

        private void PhaseChanged() => 
            PhaseChanged(_stateMachine.CurrentPhase);

        private void PhaseChanged(IGamePhase? phase) => 
            phase?.Accept(this);
    }
}