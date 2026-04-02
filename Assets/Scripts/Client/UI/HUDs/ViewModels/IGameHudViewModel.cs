using System;
using Client.UI.Dialogs.Game.Hand.ViewModels;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudViewModel : IDisposable
    {
        IGameHudTopViewModel TopViewModel { get; }
        
        IGamePlayerHandViewModel PlayerHandViewModel { get; }
        
        IReactivityProperty<IGameDestinyCardViewModel> DestinyCardViewModel { get; }
    }
}