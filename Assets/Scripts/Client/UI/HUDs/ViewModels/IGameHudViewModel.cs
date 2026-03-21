using Client.UI.Dialogs.Game.Hand.ViewModels;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudViewModel
    {
        IGamePlayerHandViewModel PlayerHandViewModel { get; }
        
        IReactivityProperty<string> PhaseName { get; }
    }
}