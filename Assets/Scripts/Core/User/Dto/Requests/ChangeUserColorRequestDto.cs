using System;

namespace Core.User.Dto.Requests
{
    [Serializable]
    public class ChangeUserColorRequestDto
    {
        public ulong UserId;

        public int SelectedColorId;
    }
}