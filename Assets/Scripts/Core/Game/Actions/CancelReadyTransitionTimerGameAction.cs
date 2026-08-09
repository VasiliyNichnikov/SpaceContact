using System;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CancelReadyTransitionTimerGameAction : BaseGameAction
    {
        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            var phase = context.Phase;

            if (phase.EndTimeInSeconds == null)
            {
                return GameActionResult.Failed("The EndTimeInSeconds phase is null.");
            }

            if (phase.EndTimeInSeconds > context.Time.ServerTimeInSeconds)
            {
                return GameActionResult.Failed("The phase has already been completed.");
            }

            if (phase.ReadyTransitionTimeInSeconds == null)
            {
                return GameActionResult.Failed("The timer was not running.");
            }
            
            return GameActionResult.Success(new ApplyPhaseTransitionTimerCancellationGameEvent());
        }
    }
}