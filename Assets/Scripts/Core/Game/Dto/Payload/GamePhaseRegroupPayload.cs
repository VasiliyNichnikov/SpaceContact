using System;
using Core.Game.Phases;

namespace Core.Game.Dto.Payload
{
    [Serializable]
    public class GamePhaseRegroupPayload : IPhasePayload
    {
        public double EndPhaseTime;

        double IPhasePayload.EndPhaseTime => 
            EndPhaseTime;
    }
}