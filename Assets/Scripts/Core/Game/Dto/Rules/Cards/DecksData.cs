using System.Collections.Generic;

namespace Core.Game.Dto.Rules.Cards
{
    public class DecksData
    {
        public readonly IReadOnlyCollection<SpaceCardDamageData> DamageSpaceCards;
        
        public readonly IReadOnlyCollection<SpaceCardArtifactData> ArtifactsSpaceCards;

        public readonly int NumberOfConversationsSpaceCards;

        public DecksData(IReadOnlyCollection<SpaceCardDamageData> damage,
            IReadOnlyCollection<SpaceCardArtifactData> artifacts,
            int numberOfConversations)
        {
            DamageSpaceCards = damage;
            ArtifactsSpaceCards = artifacts;
            NumberOfConversationsSpaceCards = numberOfConversations;
        }
    }
}