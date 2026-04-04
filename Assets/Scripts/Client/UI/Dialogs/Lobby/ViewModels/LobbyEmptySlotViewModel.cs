using System;

namespace Client.UI.Dialogs.Lobby.ViewModels
{
    public class LobbyEmptySlotViewModel : ILobbySlotViewModel
    {
        private readonly Action<int> _onChoosePlaceClickHandler;
        private readonly int _number;
        
        public LobbyEmptySlotViewModel(int number, Action<int> onChoosePlaceClickHandler)
        {
            _number = number;
            _onChoosePlaceClickHandler = onChoosePlaceClickHandler;
        }

        public void OnChoosePlaceClickHandler() => 
            _onChoosePlaceClickHandler.Invoke(_number);
    }
}