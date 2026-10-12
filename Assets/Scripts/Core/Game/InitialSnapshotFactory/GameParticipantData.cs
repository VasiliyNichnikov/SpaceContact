using Core.EngineData;

namespace Core.Game.InitialSnapshotFactory
{
    public readonly struct GameParticipantData
    {
        public GameParticipantData(ulong playerId, string name, int seatNumber, Color color)
        {
            PlayerId = playerId;
            Name = name;
            SeatNumber = seatNumber;
            Color = color;
        }

        public ulong PlayerId { get; }

        public string Name { get; }

        public int SeatNumber { get; }

        public Color Color { get; }
    }
}