using System;
using Core.Phases;

namespace Core.Factory
{
    public interface IPhaseFactory
    {
        IGamePhase Create(Type phaseType, IPhasePayload? payload);
    }
}