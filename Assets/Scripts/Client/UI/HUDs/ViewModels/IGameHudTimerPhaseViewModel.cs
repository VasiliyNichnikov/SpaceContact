using System;
using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudTimerPhaseViewModel : IDisposable
    {
        IReactivityProperty<int> RemainingTimeInSeconds { get; }

        void Update();
    }
}