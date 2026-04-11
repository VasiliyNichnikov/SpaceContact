using System;

namespace Network.Dto.Requests
{
    [Serializable]
    public class ChangeUserNameRequestDto
    {
        public ulong UserId;
        
        public string Name = null!;
    }
}