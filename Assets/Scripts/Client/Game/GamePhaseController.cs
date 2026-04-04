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
        private readonly GameFieldPlanetsViewProvider _fieldPlanetsViewProvider;
        private readonly GameUILoader _gameUILoader;
        private readonly IGameFieldViewManager _fieldViewManager;
        
        public GamePhaseController(
            IGameStateMachineReadOnly stateMachine, 
            GameFieldPlanetsViewProvider fieldPlanetsViewProvider,
            GameUILoader gameUILoader,
            IGameFieldViewManager fieldViewManager)
        {
            _stateMachine = stateMachine;
            _fieldPlanetsViewProvider = fieldPlanetsViewProvider;
            _gameUILoader = gameUILoader;
            _fieldViewManager = fieldViewManager;
            _stateMachine.OnPhaseChanged += PhaseChanged;
        }
        
        void IPhaseVisitor.Visit(GameInitializationPhase phase)
        {
            _fieldPlanetsViewProvider.CreateFieldPlanets();
            // После инициализации планет
            _fieldViewManager.Init();
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