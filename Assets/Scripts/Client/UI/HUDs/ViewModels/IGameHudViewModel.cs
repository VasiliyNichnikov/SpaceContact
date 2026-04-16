using System;
using System.Collections.Generic;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudViewModel : IDisposable
    {
        IGameHudTopViewModel TopViewModel { get; }
        
        IGameHudBottomViewModel BottomViewModel { get; }
        
        IReactivityProperty<IGameDestinyCardViewModel?> DestinyCardViewModel { get; }
        
        IReactivityProperty<GamePlayerBlockViewModel> OpponentPlayerViewModel { get; }
        
        IReadOnlyCollection<GamePlayerProfileViewModel> PlayerProfilesViewModels { get; }
    }
}