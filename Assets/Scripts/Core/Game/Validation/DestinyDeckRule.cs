using Core.Game.Dto.Snapshots;

namespace Core.Game.Validation
{
    public sealed class DestinyDeckRule : StateValidationRule<CardsSnapshot>
    {
        protected override void Validate(CardsSnapshot value, StateValidationContext context)
        {
            var deck = value.DestinyDeck;
            if (deck is null)
            {
                return;
            }

            context.At(".DestinyDeckCount").Require(value.DestinyDeckCount == deck.Count,
                "The declared Destiny deck count disagrees with its cards.");

            for (var i = 0; i < deck.Count; i++)
            {
                context.At($".DestinyDeck[{i}]").Require(!deck[i].SpecificCardId.HasValue,
                    "Specific Destiny cards are not supported in the playable deck.");
            }
        }
    }
}
