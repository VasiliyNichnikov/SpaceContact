using Client.UI.Dialogs.Game.Hand.ViewModels;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudViewModel
    {
        public IGamePlayerHandViewModel PlayerHandViewModel { get; }
    }
}