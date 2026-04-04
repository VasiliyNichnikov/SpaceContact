using System;

namespace Core.User.Dto.Requests
{
    [Serializable]
    public class ChangeUserSeatNumberRequestDto
    {
        public ulong UserId;
        
        public int SeatNumber;
    }
}