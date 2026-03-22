using System;
using System.Collections.Generic;
using Client.Helpers;
using Core.Game;
using Core.Game.Phases;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public class GameHudTopViewModel : IGameHudTopViewModel, IDisposable
    {
        private readonly EventProvider _activePhaseChangedEvent = new();
        private readonly IGameStateMachineReadOnly _stateMachine;
        private readonly PhasesHelper _phasesHelper;
        
        public GameHudTopViewModel(IGameStateMachineReadOnly stateMachine, PhasesHelper phasesHelper)
        {
            _stateMachine = stateMachine;
            _phasesHelper = phasesHelper;
            ActiveGamePhaseType = GamePhaseType.None;
            PhaseScale = Array.Empty<GamePhaseType>();
            _stateMachine.OnPhaseChanged += OnPhaseChanged;
        }

        public IEventProvider ActivePhaseChangedEvent => 
            _activePhaseChangedEvent;
        
        public GamePhaseType ActiveGamePhaseType { get; private set; }
        
        public IReadOnlyCollection<GamePhaseType> PhaseScale { get; private set; }
        
        public void Dispose() => 
            _stateMachine.OnPhaseChanged -= OnPhaseChanged;
        
        private void OnPhaseChanged()
        {
            var phase = _stateMachine.CurrentPhase;

            if (phase == null)
            {
                return;
            }

            if (!_phasesHelper.ContainsPhase(phase.Type))
            {
                return;
            }
            
            ActiveGamePhaseType = phase.Type;
            PhaseScale = _phasesHelper.GetPhaseScaleByPhase(phase.Type);
            
            _activePhaseChangedEvent.Call();
        }
    }
}