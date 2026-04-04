using System;

namespace Core.User.Dto.Requests
{
    [Serializable]
    public class ChangeUserNameRequestDto
    {
        public ulong UserId;
        
        public string Name = null!;
    }
}