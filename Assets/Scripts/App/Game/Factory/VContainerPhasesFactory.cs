using System;
using Core.Game.Factory;
using Core.Game.Phases;
using VContainer;

namespace App.Game.Factory
{
    public class VContainerPhasesFactory : IPhaseFactory
    {
        private readonly IObjectResolver _resolver;

        public VContainerPhasesFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        
        public IGamePhase Create(Type phaseType, IPhasePayload? payload)
        {
            var phase = (IGamePhase)_resolver.Resolve(phaseType);

            if (payload == null)
            {
                return phase;
            }
            
            if (phase is IPhaseWithContext<IPhasePayload> phaseWithContext)
            {
                phaseWithContext.SetContext(payload);
            }

            return phase;
        }
    }
}