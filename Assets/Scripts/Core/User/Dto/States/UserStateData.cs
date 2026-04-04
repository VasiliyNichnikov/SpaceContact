using System;

namespace Core.User.Dto.States
{
    [Serializable]
    public class UserStateData
    {
        public ulong UserId;
        
        public string Name = null!;

        public bool IsOwnerLobby;

        public int ColorId;

        public int SeatNumber;
    }
}