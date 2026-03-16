using System;

namespace Core.Game.Dto.Requests
{
    [Serializable]
    public class PlayerHandStateRequestDto
    {
        public ulong PlayerId { get; set; }
        
        public bool OnlyNumberCardsInHand { get; set; }
    }
}