using Client.UI.Dialogs.Game.Hand.ViewModels;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudViewModel
    {
        IGameHudTopViewModel TopViewModel { get; }
        
        IGamePlayerHandViewModel PlayerHandViewModel { get; }
    }
}