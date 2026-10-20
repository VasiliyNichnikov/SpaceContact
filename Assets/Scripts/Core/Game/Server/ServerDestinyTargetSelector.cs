using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Actions;
using Core.Game.Dto.Snapshots;

namespace Core.Game.Server
{
    public sealed class ServerDestinyTargetSelector
    {
        private readonly Random _random;

        public ServerDestinyTargetSelector(Random random)
        {
            _random = random;
        }

        public SelectTargetPlanetGameAction? SelectTarget(GameContext context)
        {
            if (context.Role != GameContextRole.Server || context.Phase.Type != GamePhaseType.Destiny ||
                context.Encounter.HasPlanetToAttack)
            {
                return null;
            }

            var aggressorPlayerId = context.Encounter.AggressorPlayerId;
            if (!aggressorPlayerId.HasValue)
            {
                return null;
            }

            var actor = GameActor.Player(aggressorPlayerId.Value);
            var candidatesByDefender = new List<List<SelectTargetPlanetGameAction>>();
            var planets = context.Galaxy.Planets.OrderBy(planet => planet.Id).ToArray();
            foreach (var player in context.Players.All.OrderBy(player => player.Order))
            {
                if (player.Id == context.Encounter.AggressorPlayerId)
                {
                    continue;
                }

                var candidates = new List<SelectTargetPlanetGameAction>();
                foreach (var planet in planets)
                {
                    var action = new SelectTargetPlanetGameAction(planet.Id, player.Id);
                    if (action.GetValidator().Check(context, actor).IsSuccess)
                    {
                        candidates.Add(action);
                    }
                }

                if (candidates.Count > 0)
                {
                    candidatesByDefender.Add(candidates);
                }
            }

            if (candidatesByDefender.Count == 0)
            {
                return null;
            }

            // Choose the defender first so players with more planets are not more likely to be selected.
            var selectedCandidates = candidatesByDefender[_random.Next(candidatesByDefender.Count)];
            return selectedCandidates[_random.Next(selectedCandidates.Count)];
        }
    }
}
