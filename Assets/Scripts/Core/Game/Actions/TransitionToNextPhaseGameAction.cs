using System;
using System.Linq;
using Core.Game.Dto.Snapshots;
using Core.Game.Events;
using Core.Helpers;
using Newtonsoft.Json;

namespace Core.Game.Actions
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class TransitionToNextPhaseGameAction : BaseGameAction
    {
        public TransitionToNextPhaseGameAction(GamePhaseType currentPhaseType, long currentPhaseRevision,
            GamePhaseType nextPhaseType, long phaseRevision)
        {
            CurrentPhaseType = currentPhaseType;
            CurrentPhaseRevision = currentPhaseRevision;
            NextPhaseType = nextPhaseType;
            PhaseRevision = phaseRevision;
        }

        [JsonProperty]
        public GamePhaseType CurrentPhaseType { get; }

        [JsonProperty]
        public long CurrentPhaseRevision { get; }

        [JsonProperty]
        public GamePhaseType NextPhaseType { get; }

        [JsonProperty]
        public long PhaseRevision { get; }

        public override GameActionResult Execute(GameContext context, GameActor actor)
        {
            if (!ValidateAccess(context, actor, out var error) ||
                !ValidateCurrentPhase(context.Phase, out error) ||
                !ValidateNewRevision(context, out error) ||
                !ValidateTransition(context.Phase, out error) ||
                !ValidateParticipants(context, out error) ||
                !ValidateTransitionTime(context, out error))
            {
                return GameActionResult.Failed(error);
            }

            return GameActionResult.Success(new PhaseChangedGameEvent(NextPhaseType, PhaseRevision));
        }

        private static bool ValidateAccess(GameContext context, GameActor actor, out string error)
        {
            if (context.Role != GameContextRole.Server || !actor.IsSystem)
            {
                error = "Only the system can transition phases on the server.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool ValidateParticipants(GameContext context, out string error)
        {
            if (context.Players.All.Count == 0)
            {
                error = "A phase transition requires participants.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool ValidateTransitionTime(GameContext context, out string error)
        {
            error = string.Empty;
            var phase = context.Phase;
            if (!TransitionPhaseHelper.IsTimedPhase(phase.Type))
            {
                return true;
            }

            if (!phase.EndTimeInSeconds.HasValue)
            {
                error = "The phase timer has not started.";
                return false;
            }

            var now = context.Time.ServerTimeInSeconds;
            var endTime = phase.EndTimeInSeconds.Value;
            var readyTime = phase.ReadyTransitionTimeInSeconds;
            if (now < endTime && !(readyTime.HasValue && now >= readyTime.Value && context.Players.All.All(p => p.IsReady)))
            {
                error = "Neither the phase nor the ready deadline has elapsed.";
                return false;
            }

            return true;
        }

        private bool ValidateCurrentPhase(PhaseContext phase, out string error)
        {
            if (phase.Type != CurrentPhaseType || phase.Revision != CurrentPhaseRevision)
            {
                error = "The source phase or its revision is stale.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private bool ValidateNewRevision(GameContext context, out string error)
        {
            if (context.Revision == long.MaxValue || PhaseRevision != context.Revision + 1 || PhaseRevision <= context.Phase.Revision)
            {
                error = "The new phase revision must identify the next state commit.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private bool ValidateTransition(PhaseContext phase, out string error)
        {
            if (!TransitionPhaseHelper.TryGetNextPhase(phase.Type, out var nextPhase) || nextPhase != NextPhaseType)
            {
                error = "The requested phase transition is not supported.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
