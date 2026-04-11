using Client.UI.Dialogs.Game.Hand.ViewModels;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudBottomViewModel
    {
        GameHudBottomSwitchesViewModel SwitchesViewModel { get; }
        
        IGamePlayerHandViewModel PlayerHandViewModel { get; }
    }
}