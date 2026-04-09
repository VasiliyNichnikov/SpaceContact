using System;

namespace Core.Game.Dto.States.Cards
{
    [Serializable]
    public struct DestinyCardData
    {
        private const int ErrorIntValue = int.MinValue;
        private const ulong ErrorLongValue = ulong.MinValue;
        
        public bool IsJoker;

        public bool IsColorCard;
        
        public ulong SelectedPlayerId;

        public bool IsSpecificCard;
        
        public int? SpecificCardId;

        private DestinyCardData(
            bool isJoker,
            bool isColorCard,
            ulong selectedPlayerId,
            bool isSpecificCard,
            int specificCardId)
        {
            IsJoker = isJoker;
            IsColorCard = isColorCard;
            SelectedPlayerId = selectedPlayerId;
            IsSpecificCard = isSpecificCard;
            SpecificCardId = specificCardId;
        }
        
        public static DestinyCardData JokerCard()
        {
            return new DestinyCardData(
                true, 
                false, 
                ErrorLongValue, 
                false, 
                ErrorIntValue);
        }
        
        public static DestinyCardData ColorCard(ulong selectedPlayerId)
        {
            return new DestinyCardData(
                false, 
                true, 
                selectedPlayerId, 
                false, 
                ErrorIntValue);
        }
        
        public static DestinyCardData SpecificCard(int specificCardId)
        {
            return new DestinyCardData(
                false, 
                false, 
                ErrorLongValue, 
                true, 
                specificCardId);
        } 
    }
}