using Client.UI.Dialogs.Game.Hand.ViewModels;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudBottomViewModel : IGameHudBottomViewModel
    {
        public GameHudBottomViewModel(
            IGamePlayerHandViewModel playerHandViewModel, 
            GameHudBottomSwitchesViewModel switchesViewModel)
        {
            PlayerHandViewModel = playerHandViewModel;
            SwitchesViewModel = switchesViewModel;
        }

        public GameHudBottomSwitchesViewModel SwitchesViewModel { get; }

        public IGamePlayerHandViewModel PlayerHandViewModel { get; }
    }
}
