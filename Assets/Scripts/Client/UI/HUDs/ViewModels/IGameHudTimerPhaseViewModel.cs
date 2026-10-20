using System;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudTimerPhaseViewModel : IDisposable
    {
        IReactivityProperty<int> RemainingTimeInSeconds { get; }

        IReactivityProperty<bool> IsReadyToNextPhase { get; }

        IReactivityProperty<bool> CanToggleReady { get; }

        void OnReadyButtonClickHandler();

        void Update();

        void Init();
    }
}
