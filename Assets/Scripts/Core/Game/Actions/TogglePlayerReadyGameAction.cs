using System;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class TogglePlayerReadyGameAction : BaseGameAction
    {
        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            var selectedPlayerId = actor.PlayerId;

            if (selectedPlayerId == null)
            {
                return GameActionResult.Failed("Player to switch is null");
            }

            var selectedPlayer = context.Players.Get(selectedPlayerId.Value);
            var gameEvent = new PlayerReadyToggledGameEvent(selectedPlayer.PlayerId, !selectedPlayer.IsReady);

            return GameActionResult.Success(gameEvent);
        }
    }
}