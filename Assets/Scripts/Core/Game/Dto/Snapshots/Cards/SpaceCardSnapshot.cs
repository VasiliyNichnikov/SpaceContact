using System;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    public struct SpaceCardSnapshot
    {
        public int? DamageCount;

        public int? ArtifactId;
        
        public bool IsConversation;
        
        public static SpaceCardSnapshot ConversationStateCard()
        {
            return new SpaceCardSnapshot
            {
                DamageCount = null,
                ArtifactId = null,
                IsConversation = true
            };
        }

        public static SpaceCardSnapshot DamageStateCard(int damageCount)
        {
            return new SpaceCardSnapshot
            {
                DamageCount = damageCount,
                ArtifactId = null,
                IsConversation = false
            };
        }

        public static SpaceCardSnapshot ArtifactStateCard(int artifactId)
        {
            return new SpaceCardSnapshot
            {
                DamageCount = null,
                ArtifactId = artifactId,
                IsConversation = false
            };
        }
    }
}