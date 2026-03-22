using System.Collections.Generic;
using Core.Game.Phases;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudTopViewModel
    {
        IEventProvider ActivePhaseChangedEvent { get; }
        
        GamePhaseType ActiveGamePhaseType { get; }
        
        IReadOnlyCollection<GamePhaseType> PhaseScale { get; }
    }
}