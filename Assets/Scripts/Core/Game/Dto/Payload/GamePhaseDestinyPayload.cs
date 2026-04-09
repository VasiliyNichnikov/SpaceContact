using System;
using Core.Game.Dto.States;
using Core.Game.Phases;

namespace Core.Game.Dto.Payload
{
    [Serializable]
    public class GamePhaseDestinyPayload : IPhasePayload
    {
        public EncounterStateData EncounterState = null!;
    }
}