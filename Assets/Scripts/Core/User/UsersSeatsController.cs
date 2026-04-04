using System;
using System.Linq;
using Core.Lobby.Dto;
using Logs;

namespace Core.User
{
    public sealed class UsersSeatsController : IUsersSeatsProvider
    {
        private readonly int _maxNumberOfSeats;
        private readonly ulong?[] _seats;
        
        public UsersSeatsController(LobbySettingsData settingsData)
        {
            _maxNumberOfSeats = settingsData.MaxNumberOfPlayers;
            _seats = new ulong?[_maxNumberOfSeats];
        }
        
        public event Action? SlotsChanged;

        public ulong?[] Seats => _seats;
        
        public bool IsSeatAvailable(int seatNumber)
        {
            if (seatNumber < 1 || seatNumber > _maxNumberOfSeats)
            {
                return false;
            }
            
            return _seats[seatNumber - 1] == null;
        }

        public int GetFreeSeatNumber()
        {
            var firstEmptyIndex = Array.FindIndex(_seats, u => u == null);

            if (firstEmptyIndex < 0)
            {
                throw new IndexOutOfRangeException("No seats found");
            }

            return firstEmptyIndex + 1;
        }
        
        public void AttachUserToSeatNumber(ulong userId, int seatNumber)
        {
            if (_seats.Contains(userId))
            {
                Logger.Error("UsersSeatsController.AttachUserToSeatNumber: user id is already attached.");
                return;
            }

            if (seatNumber < 1 || seatNumber > _maxNumberOfSeats)
            {
                Logger.Error("UsersSeatsController.AttachUserToSeatNumber: seatNumber is out of range.");
                return;
            }

            var seatIndex = seatNumber - 1;
            _seats[seatIndex] = userId;

            SlotsChanged?.Invoke();
        }

        public void DetachUserFromSeatNumber(ulong userId)
        {
            if (!_seats.Contains(userId))
            {
                Logger.Error($"UsersSeatsController.DetachUserFromSeatNumber: user with id {userId} is not found.");
                return;
            }
            
            var seatIndex = Array.FindIndex(_seats, u => u == userId);
            _seats[seatIndex] = null;
            
            SlotsChanged?.Invoke();
        }

        public bool TryChangeSeatNumber(ulong userId, int newSeatNumber)
        {
            if (!_seats.Contains(userId))
            {
                Logger.Error($"UsersSeatsController.TryChangeSeatNumber: user with id {userId} is not found.");
                return false;
            }

            var newSeatIndex = newSeatNumber - 1;
            
            if (_seats[newSeatIndex] != null)
            {
                Logger.Error("UsersSeatsController.TryChangeSeatNumber: the selected seat is occupied.");
                return false;
            }
            
            var oldSeatIndex = Array.FindIndex(_seats, u => u == userId);
            _seats[oldSeatIndex] = null;
            _seats[newSeatIndex] = userId;
            
            SlotsChanged?.Invoke();
            return true;
        }
    }
}