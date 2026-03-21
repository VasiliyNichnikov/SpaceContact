namespace Client.Data.Game
{
    public readonly struct PlayerHandDisplayData
    {
        public readonly float ArchWidth;
        
        public readonly float ArchHeight;

        public readonly float MaxRotation;

        public PlayerHandDisplayData(
            float archWidth,
            float archHeight,
            float maxRotation)
        {
            ArchWidth = archWidth;
            ArchHeight = archHeight;
            MaxRotation = maxRotation;
        }
    }
}