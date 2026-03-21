using System;

namespace Core.Game.Dto.Requests
{
    [Serializable]
    public class PlayerHandStateRequestDto
    {
        public ulong PlayerId;

        public bool OnlyNumberCardsInHand;
    }
}