using System;
using System.Collections.Generic;
using Client.Helpers;
using Core.Game;
using Core.Game.Dto.Snapshots;
using Reactivity;
using VContainer.Unity;

namespace Client.UI.HUDs.ViewModels
{
    public class GameHudTopViewModel : IGameHudTopViewModel, IInitializable, IDisposable
    {
        private readonly ClientGameContext _context;
        private readonly EventProvider _activePhaseChangedEvent = new();
        private readonly PhasesHelper _phasesHelper;

        public GameHudTopViewModel(ClientGameContext context, PhasesHelper phasesHelper)
        {
            _context = context;
            _phasesHelper = phasesHelper;
        }

        public IEventProvider ActivePhaseChangedEvent => _activePhaseChangedEvent;

        public GamePhaseType ActiveGamePhaseType { get; private set; } = GamePhaseType.None;

        public IReadOnlyCollection<GamePhaseType> PhaseScale { get; private set; } = Array.Empty<GamePhaseType>();

        public void Dispose()
        {
            _context.StateChanged -= Refresh;
        }

        void IInitializable.Initialize()
        {
            _context.StateChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            var phaseType = _context.IsInitialized ? _context.Phase.Type : GamePhaseType.None;
           
            if (ActiveGamePhaseType == phaseType)
            {
                return;
            }

            ActiveGamePhaseType = phaseType;
            PhaseScale = _phasesHelper.ContainsPhase(phaseType)
                ? _phasesHelper.GetPhaseScaleByPhase(phaseType)
                : Array.Empty<GamePhaseType>();
            _activePhaseChangedEvent.Call();
        }
    }
}
