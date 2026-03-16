using System;

namespace Core.Game.Dto.States.Cards
{
    [Serializable]
    public struct SpaceCardStateData
    {
        private const int InvalidValue = int.MinValue;
        
        public int DamageCount;
        
        public bool IsDamage;

        public int ArtifactId;

        public bool IsArtifact;
        
        public bool IsConversation;
        
        public static SpaceCardStateData ConversationStateCard()
        {
            return new SpaceCardStateData
            {
                DamageCount = InvalidValue,
                IsDamage = false,
                ArtifactId = InvalidValue,
                IsArtifact = false,
                IsConversation = true
            };
        }

        public static SpaceCardStateData DamageStateCard(int damageCount)
        {
            return new SpaceCardStateData
            {
                DamageCount = damageCount,
                IsDamage = true,
                ArtifactId = InvalidValue,
                IsArtifact = false,
                IsConversation = false
            };
        }

        public static SpaceCardStateData ArtifactStateCard(int artifactId)
        {
            return new SpaceCardStateData
            {
                DamageCount = InvalidValue,
                IsDamage = false,
                ArtifactId = artifactId,
                IsArtifact = true,
                IsConversation = false
            };
        }
    }
}