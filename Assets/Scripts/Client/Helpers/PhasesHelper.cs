using System;
using System.Collections.Generic;
using Core.Game.Phases;
using Logs;

namespace Client.Helpers
{
    public sealed class PhasesHelper
    {
        /// <summary>
        /// Последовательность фаз важна
        /// </summary>
        private readonly List<GamePhaseType> _visiblePhases = new()
        {
            GamePhaseType.Regroup,
            GamePhaseType.Destiny,
            GamePhaseType.Launch,
            GamePhaseType.Alliance
        };

        public IReadOnlyCollection<GamePhaseType> GetPhaseScaleByPhase(GamePhaseType phase)
        {
            var currentIndex = _visiblePhases.IndexOf(phase);

            if (currentIndex < 0)
            {
                Logger.Error($"PhasesHelper.GetPhaseScaleByPhase: {phase} not found.");
                
                return ArraySegment<GamePhaseType>.Empty;
            }
            
            var result = new List<GamePhaseType>(3);

            if (currentIndex == 0)
            {
                result.Add(_visiblePhases[0]);
                result.Add(_visiblePhases[1]);
                result.Add(_visiblePhases[2]);
            }
            else if (currentIndex + 1 >= _visiblePhases.Count)
            {
                result.Add(_visiblePhases[currentIndex - 1]);
                result.Add(_visiblePhases[currentIndex]);
                result.Add(_visiblePhases[0]);
            }
            else
            {
                result.Add(_visiblePhases[currentIndex - 1]);
                result.Add(_visiblePhases[currentIndex]);
                result.Add(_visiblePhases[currentIndex + 1]);
            }

            return result;
        }
        
        public bool ContainsPhase(GamePhaseType phase) => 
            _visiblePhases.Contains(phase);
    }
}