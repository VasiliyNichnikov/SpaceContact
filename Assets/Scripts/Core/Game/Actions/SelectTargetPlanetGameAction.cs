using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SelectTargetPlanetGameAction : BaseGameAction
    {
        public SelectTargetPlanetGameAction(int planetId)
        {
            PlanetId = planetId;
        }
        
        [JsonProperty]
        public int PlanetId { get; }
        
        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (actor.PlayerId == null)
            {
                return GameActionResult.Failed("Actor playerId is null.");
            }

            var destinyCard = context.Cards.ActiveDestinyCard;

            if (destinyCard == null)
            {
                return GameActionResult.Failed("DestinyCard is null.");
            }

            var planetData = context.Galaxy.Planets.FirstOrDefault(p => p.Id == PlanetId);

            if (planetData == null)
            {
                return GameActionResult.Failed($"Planet with id {PlanetId} not found in Galaxy.");
            }
            
            var events = new List<IGameEvent>();
            var defenderPlayerId = context.Encounter.DefenderPlayerId;
            
            if (destinyCard.IsJoker)
            {
                if (defenderPlayerId != null)
                {
                    return GameActionResult.Failed("When joker destiny active, the defender must be empty.");
                }

                var planetOwnerId = context.Galaxy.GetPlanetOwnerId(planetData.Id);
                events.Add(new ApplyDefenderPlayerGameEvent(planetOwnerId));
            }
            
            events.Add(new ApplyPlanetToAttackGameEvent(PlanetId));
            return GameActionResult.Success(events);
        }
    }
}