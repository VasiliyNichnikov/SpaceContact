using System;

namespace Network.Dto.Requests
{
    [Serializable]
    public class ChangeUserSeatNumberRequestDto
    {
        public ulong UserId;
        
        public int SeatNumber;
    }
}