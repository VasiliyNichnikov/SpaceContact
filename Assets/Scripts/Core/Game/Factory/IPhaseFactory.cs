using System;
using Core.Game.Phases;

namespace Core.Game.Factory
{
    public interface IPhaseFactory
    {
        IGamePhase Create(Type phaseType, IPhasePayload? payload);
    }
}