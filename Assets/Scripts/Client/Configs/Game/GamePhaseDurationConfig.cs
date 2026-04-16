using Core.Game.Dto.Phases;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "GamePhaseDurationConfig", menuName = "Configs/Game/GamePhaseDurationConfig", order = 0)]
    public sealed class GamePhaseDurationConfig : ScriptableObject
    {
        [SerializeField, Min(0), Header("In Seconds")] 
        private float _regroupPhaseDuration;
        
        [SerializeField, Min(0), Header("In Seconds")]
        private float _destinyPhaseDuration;
        
        [SerializeField, Min(0), Header("In Seconds")]
        private float _launchPhaseDuration;
        
        [SerializeField, Min(0), Header("In Seconds")]
        private float _alliancePhaseDuration;
        
        public GamePhaseDurationData BuildData()
        {
            return new GamePhaseDurationData(
                _regroupPhaseDuration, 
                _destinyPhaseDuration, 
                _launchPhaseDuration,
                _alliancePhaseDuration);
        }
    }
}