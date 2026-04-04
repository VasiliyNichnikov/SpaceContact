using System;

namespace Core.User
{
    public interface IUser
    {
        ulong ClientId { get; }
        
        string Name { get; }
        
        int ColorId { get; }
        
        bool IsCurrentPlayer { get; }
        
        bool IsOwnerLobby { get; }
        
        int SeatNumber { get; }

        event Action? Changed;
    }
}