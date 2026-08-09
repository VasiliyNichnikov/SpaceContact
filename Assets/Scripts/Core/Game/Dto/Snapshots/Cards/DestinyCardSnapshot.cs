using System;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    public struct DestinyCardSnapshot
    {
        public bool IsJoker;
        
        public ulong? SelectedPlayerId;
        
        public int? SpecificCardId;

        private DestinyCardSnapshot(
            bool isJoker,
            ulong? selectedPlayerId,
            int? specificCardId)
        {
            IsJoker = isJoker;
            SelectedPlayerId = selectedPlayerId;
            SpecificCardId = specificCardId;
        }
        
        public static DestinyCardSnapshot JokerCard()
        {
            return new DestinyCardSnapshot(
                true, 
                null, 
                null);
        }
        
        public static DestinyCardSnapshot ColorCard(ulong selectedPlayerId)
        {
            return new DestinyCardSnapshot(
                false, 
                selectedPlayerId, 
                null);
        }
        
        public static DestinyCardSnapshot SpecificCard(int specificCardId)
        {
            return new DestinyCardSnapshot(
                false,
                null,
                specificCardId);
        } 
    }
}