using Core.Game.Dto.States.Cards;
using Logs;

namespace Core.Game.Cards
{
    public class SpaceCardFactory
    {
        public ISpaceCard Create(SpaceCardStateData state)
        {
            if (state.IsDamage)
            {
                return new DamageSpaceCard(state.DamageCount);
            }

            if (state.IsArtifact)
            {
                return new ArtifactSpaceCard();
            }

            if (state.IsConversation)
            {
                return new ConversationSpaceCard();
            }
            
            Logger.Error("SpaceCardFactory.Create: incorrect state card.");
            
            return EmptySpaceCard.Instance;
        }
    }
}