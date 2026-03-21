using System;

namespace Core.Game.Dto.States.Cards
{
    [Serializable]
    public struct DestinyCardStateData
    {
        private const int ErrorIntValue = int.MinValue;
        private const ulong ErrorLongValue = ulong.MinValue;
        
        public bool IsJoker;

        public bool IsColorCard;
        
        public ulong SelectedPlayerId;

        public bool IsSpecificCard;
        
        public int? SpecificCardId;

        private DestinyCardStateData(
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
        
        public static DestinyCardStateData JokerCard()
        {
            return new DestinyCardStateData(
                true, 
                false, 
                ErrorLongValue, 
                false, 
                ErrorIntValue);
        }
        
        public static DestinyCardStateData ColorCard(ulong selectedPlayerId)
        {
            return new DestinyCardStateData(
                false, 
                true, 
                selectedPlayerId, 
                false, 
                ErrorIntValue);
        }
        
        public static DestinyCardStateData SpecificCard(int specificCardId)
        {
            return new DestinyCardStateData(
                false, 
                false, 
                ErrorLongValue, 
                true, 
                specificCardId);
        } 
    }
}