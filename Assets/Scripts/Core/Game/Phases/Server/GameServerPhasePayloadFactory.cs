using Core.Game.Dto.Payload;
using Core.Game.Dto.Phases;
using Core.Game.Encounter;

namespace Core.Game.Phases.Server
{
    public sealed class GameServerPhasePayloadFactory
    {
        private readonly GamePhaseDurationData _phaseDurationData;
        private readonly IGameServerTime _serverTime;
        private readonly IGameServerEncounterManager _serverEncounterManager;
        
        public GameServerPhasePayloadFactory(
            GamePhaseDurationData phaseDurationData,
            IGameServerTime serverTime,
            IGameServerEncounterManager serverEncounterManager)
        {
            _phaseDurationData = phaseDurationData;
            _serverTime = serverTime;
            _serverEncounterManager = serverEncounterManager;
        }
        
        public GamePhaseDestinyPayload CreateDestinyPayload()
        {
            var endPhaseTime = CalculateEndTimePhase(_phaseDurationData.DestinyPhaseDuration);
            var encounterState = _serverEncounterManager.ToState();

            return new GamePhaseDestinyPayload
            {
                EncounterState = encounterState,
                EndPhaseTime = endPhaseTime
            };
        }

        public GamePhaseRegroupPayload CreateRegroupPayload()
        {
            var endPhaseTime = CalculateEndTimePhase(_phaseDurationData.RegroupPhaseDuration);

            return new GamePhaseRegroupPayload
            {
                EndPhaseTime = endPhaseTime
            };
        }

        private double CalculateEndTimePhase(double phaseDuration) => 
            _serverTime.ServerTimeInSeconds + phaseDuration;
    }
}