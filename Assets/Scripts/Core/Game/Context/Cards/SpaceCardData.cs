using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class SpaceCardData
    {
        public SpaceCardData(int? damageCount, int? artifactId, bool isConversation)
        {
            DamageCount = damageCount;
            ArtifactId = artifactId;
            IsConversation = isConversation;
        }

        public int? DamageCount { get; }

        public int? ArtifactId { get; }

        public bool IsConversation { get; }

        public static SpaceCardData Convert(SpaceCardSnapshot snapshot)
        {
            return new SpaceCardData(snapshot.DamageCount, snapshot.ArtifactId, snapshot.IsConversation);
        }

        public SpaceCardSnapshot ToSnapshot()
        {
            return new SpaceCardSnapshot
            {
                DamageCount = DamageCount,
                ArtifactId = ArtifactId,
                IsConversation = IsConversation
            };
        }
    }
}
