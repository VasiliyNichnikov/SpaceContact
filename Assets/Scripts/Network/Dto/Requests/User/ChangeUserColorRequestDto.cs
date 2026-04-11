using System;

namespace Network.Dto.Requests
{
    [Serializable]
    public class ChangeUserColorRequestDto
    {
        public ulong UserId;

        public int SelectedColorId;
    }
}