using System;
using Client.Game.Field;
using Logs;
using Reactivity;

namespace Client.UI.Dialogs.Game.PlayerChoice.ViewModels
{
    public sealed class GameArrowsHolderViewModel : IDisposable
    {
        private readonly ReactivityProperty<bool> _isLeftArrowVisible = new();
        private readonly ReactivityProperty<bool> _isRightArrowVisible = new();

        private readonly IGameFieldViewManager _fieldViewManager;

        public GameArrowsHolderViewModel(IGameFieldViewManager fieldViewManager)
        {
            _fieldViewManager = fieldViewManager;
        }

        public IReactivityProperty<bool> IsLeftArrowVisible =>
            _isLeftArrowVisible;

        public IReactivityProperty<bool> IsRightArrowVisible =>
            _isRightArrowVisible;

        public void Init()
        {
            _fieldViewManager.OnInitialized += RefreshArrowsVisibility;
            _fieldViewManager.OnViewedOpponentChanged += RefreshArrowsVisibility;
            RefreshArrowsVisibility();
        }

        public void Dispose()
        {
            _fieldViewManager.OnInitialized -= RefreshArrowsVisibility;
            _fieldViewManager.OnViewedOpponentChanged -= RefreshArrowsVisibility;
        }

        public void OnLeftArrowClickHandler()
        {
            if (!_fieldViewManager.CanMoveToLeftOpponent())
            {
                Logger.Error($"{nameof(GameArrowsHolderViewModel)}.{nameof(OnLeftArrowClickHandler)}: cannot move left opponent.");
                return;
            }

            _fieldViewManager.MoveToLeftOpponent();
            RefreshArrowsVisibility();
        }

        public void OnRightArrowClickHandler()
        {
            if (!_fieldViewManager.CanMoveToRightOpponent())
            {
                Logger.Error($"{nameof(GameArrowsHolderViewModel)}.{nameof(OnRightArrowClickHandler)}: cannot move right opponent.");
                return;
            }

            _fieldViewManager.MoveToRightOpponent();
            RefreshArrowsVisibility();
        }

        private void RefreshArrowsVisibility()
        {
            _isLeftArrowVisible.Value = _fieldViewManager.CanMoveToLeftOpponent();
            _isRightArrowVisible.Value = _fieldViewManager.CanMoveToRightOpponent();
        }
    }
}
