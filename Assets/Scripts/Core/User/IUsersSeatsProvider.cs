using System;

namespace Core.User
{
    public interface IUsersSeatsProvider
    {
        event Action? SlotsChanged;
        
        ulong?[] Seats { get; }
        
        bool IsSeatAvailable(int seatNumber);
    }
}