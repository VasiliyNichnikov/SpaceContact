using System;
using Core.User.Dto.States;
using Logs;

namespace Core.User
{
    public class ClientUser : IUser
    {
        private readonly ulong _localUserId;
        private UserStateData _state;
        private readonly UsersColorController _colorController;
        private readonly UsersSeatsController _seatsController;
        
        public ClientUser(
            ulong localUserId, 
            UserStateData state,
            UsersColorController colorController,
            UsersSeatsController seatsController)
        {
            _localUserId = localUserId;
            _state = state;
            _colorController = colorController;
            _seatsController = seatsController;
        }
        
        public event Action? Changed;
        
        public ulong ClientId => 
            _state.UserId;
        
        public string Name => 
            _state.Name;
        
        public int ColorId => 
            _state.ColorId;

        public bool IsCurrentPlayer => 
            _localUserId == _state.UserId;

        public bool IsOwnerLobby => 
            _state.IsOwnerLobby;
        
        public int SeatNumber => 
            _state.SeatNumber;
        
        public void Init()
        {
            _colorController.AttachUserToColor(_state.UserId, _state.ColorId);
            _seatsController.AttachUserToSeatNumber(_state.UserId, _state.SeatNumber);
        }
        
        public void Apply(UserStateData state)
        {
            var isDataApplied = true;
            
            if (_state.ColorId != state.ColorId)
            {
                isDataApplied &= _colorController.TryChangeColor(state.UserId, state.ColorId);
            }

            if (_state.SeatNumber != state.SeatNumber)
            {
                isDataApplied &= _seatsController.TryChangeSeatNumber(state.UserId, state.SeatNumber);
            }

            if (!isDataApplied)
            {
                Logger.Error("ClientUser.Apply: couldn't apply the received data.");
                return;
            }
            
            _state = state;
            Changed?.Invoke();
        }

        public void OnLeftGame()
        {
            _colorController.DetachUserFromColor(_state.UserId);
            _seatsController.DetachUserFromSeatNumber(_state.UserId);
        }
    }
}