using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct DestinyCardSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        public bool IsJoker;

        [StateVisibility(StateAudience.Everyone)]
        public ulong? SelectedPlayerId;

        [StateVisibility(StateAudience.Everyone)]
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
