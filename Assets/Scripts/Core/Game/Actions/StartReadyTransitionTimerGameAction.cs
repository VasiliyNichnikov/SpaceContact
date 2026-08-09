using System;
using Core.Game.Events;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class StartReadyTransitionTimerGameAction : BaseGameAction
    {
        public StartReadyTransitionTimerGameAction(double readyTransitionTime)
        {
            ReadyTransitionTimeInSeconds = readyTransitionTime;
        }
        
        [JsonProperty]
        public double ReadyTransitionTimeInSeconds { get; }
        
        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            var phase = context.Phase;

            if (phase.EndTimeInSeconds == null)
            {
                return GameActionResult.Failed("The EndTimeInSeconds phase is null.");
            }

            if (double.IsNaN(ReadyTransitionTimeInSeconds) || ReadyTransitionTimeInSeconds < 0)
            {
                return GameActionResult.Failed("ReadyTransitionTimeInSeconds has an incorrect value.");
            }
            
            if (phase.EndTimeInSeconds > context.Time.ServerTimeInSeconds)
            {
                return GameActionResult.Failed("The phase has already been completed.");
            }

            if (phase.EndTimeInSeconds < ReadyTransitionTimeInSeconds)
            {
                return GameActionResult.Failed("The phase will end before the timer.");
            }
            
            if (phase.ReadyTransitionTimeInSeconds != null)
            {
                return GameActionResult.Failed("The timer is already running.");
            }
            
            return GameActionResult.Success(new ApplyStartReadyTransitionTimerGameEvent(ReadyTransitionTimeInSeconds));
        }
    }
}