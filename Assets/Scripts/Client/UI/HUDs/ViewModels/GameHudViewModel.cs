using System;
using Client.UI.Dialogs.Game.Hand.ViewModels;
using Core.Game;
using Core.Game.Hands;
using Core.Game.Players;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public class GameHudViewModel : IGameHudViewModel, IDisposable
    {
        private readonly GamePlayersRegistry _registry;
        private readonly IGameStateMachineReadOnly _stateMachine;
        private readonly ReactivityProperty<string> _phaseNameText = new();
        
        public GameHudViewModel(
            GamePlayersRegistry registry, 
            IGameStateMachineReadOnly stateMachine)
        {
            _registry = registry;
            _stateMachine = stateMachine;
            PlayerHandViewModel = CreatePlayerHandViewModel();
            
            _stateMachine.OnPhaseChanged += OnPhaseChanged;
        }
        
        public IGamePlayerHandViewModel PlayerHandViewModel { get; }

        public IReactivityProperty<string> PhaseName => 
            _phaseNameText;

        private IGamePlayerHandViewModel CreatePlayerHandViewModel()
        {
            var owner = _registry.GetOwnerWithError();
            var handController = owner == null 
                ? EmptyGamePlayerHandController.Instance 
                : owner.HandController;

            return new GamePlayerHandViewModel(handController);
        }

        public void Dispose()
        {
            _stateMachine.OnPhaseChanged -= OnPhaseChanged;
        }

        private void OnPhaseChanged()
        {
            var phase = _stateMachine.CurrentPhase;

            if (phase == null)
            {
                return;
            }
            
            _phaseNameText.Value = phase.GetType().ToString();
        }
    }
}