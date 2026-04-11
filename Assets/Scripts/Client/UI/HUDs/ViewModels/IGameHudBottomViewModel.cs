using Client.UI.Dialogs.Game.Hand.ViewModels;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudBottomViewModel
    {
        IReactivityProperty<bool> IsHandVisible { get; }
        
        GameHudBottomSwitchesViewModel SwitchesViewModel { get; }
        
        IGamePlayerHandViewModel PlayerHandViewModel { get; }
    }
}