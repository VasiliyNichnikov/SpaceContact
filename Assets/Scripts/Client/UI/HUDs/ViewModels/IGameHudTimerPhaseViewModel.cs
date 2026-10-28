using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudTimerPhaseViewModel
    {
        IReactivityProperty<int> RemainingTimeInSeconds { get; }

        IReactivityProperty<bool> IsReadyToNextPhase { get; }

        IReactivityProperty<bool> CanToggleReady { get; }

        void OnReadyButtonClickHandler();
    }
}
