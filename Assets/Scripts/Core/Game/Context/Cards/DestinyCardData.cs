using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class DestinyCardData
    {
        public DestinyCardData(bool isJoker, ulong? selectedPlayer, int? specificCardId)
        {
            IsJoker = isJoker;
            SelectedPlayerId = selectedPlayer;
            SpecificCardId = specificCardId;
        }

        public bool IsJoker { get; }

        public ulong? SelectedPlayerId { get; }

        public int? SpecificCardId { get; }

        public static DestinyCardData Convert(DestinyCardSnapshot snapshot)
        {
            return new DestinyCardData(snapshot.IsJoker, snapshot.SelectedPlayerId, snapshot.SpecificCardId);
        }

        public DestinyCardSnapshot ToSnapshot()
        {
            return new DestinyCardSnapshot
            {
                IsJoker = IsJoker,
                SelectedPlayerId = SelectedPlayerId,
                SpecificCardId = SpecificCardId
            };
        }
    }
}
