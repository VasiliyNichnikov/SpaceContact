using System;
using Core.Game.Validation;
using Core.Game.Visibility;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [StateRule(typeof(SpaceCardShapeRule))]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct SpaceCardSnapshot
    {
        [StateVisibility(StateAudience.Everyone)]
        [StateRange(0)]
        public int? DamageCount;

        [StateVisibility(StateAudience.Everyone)]
        [StateRange(0)]
        public int? ArtifactId;

        [StateVisibility(StateAudience.Everyone)]
        [StateRequired]
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
