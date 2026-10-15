using System;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct SpaceCardSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        public int? DamageCount;

        [StateVisibility(StateAudience.Everyone)]
        public int? ArtifactId;

        [StateVisibility(StateAudience.Everyone)]
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
