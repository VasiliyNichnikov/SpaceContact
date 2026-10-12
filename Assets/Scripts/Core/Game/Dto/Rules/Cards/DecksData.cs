using System.Collections.Generic;

namespace Core.Game.Dto.Rules.Cards
{
    public class DecksData
    {
        public readonly IReadOnlyCollection<SpaceCardDamageData> DamageSpaceCards;

        public readonly int NumberOfConversationsSpaceCards;

        public DecksData(IReadOnlyCollection<SpaceCardDamageData> damage,
            int numberOfConversations)
        {
            DamageSpaceCards = damage;
            NumberOfConversationsSpaceCards = numberOfConversations;
        }
    }
}