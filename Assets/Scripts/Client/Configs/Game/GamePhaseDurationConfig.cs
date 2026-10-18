using Core.Game.Dto.Phases;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "GamePhaseDurationConfig", menuName = "Configs/Game/GamePhaseDurationConfig", order = 0)]
    public sealed class GamePhaseDurationConfig : ScriptableObject
    {
        [SerializeField, Min(0), Header("In Seconds")]
        private float _regroupPhaseDurationInSeconds;

        [SerializeField, Min(0), Header("In Seconds")]
        private float _destinyPhaseDurationInSeconds;

        [SerializeField, Min(0), Header("In Seconds")]
        private float _launchPhaseDurationInSeconds;

        [SerializeField, Min(0), Header("In Seconds")]
        private float _alliancePhaseDurationInSeconds;

        [SerializeField, Min(0), Header("In Seconds")]
        private float _readyTransitionDurationInSeconds;

        public GamePhaseDurationData BuildData()
        {
            return new GamePhaseDurationData(
                _regroupPhaseDurationInSeconds, 
                _destinyPhaseDurationInSeconds, 
                _launchPhaseDurationInSeconds,
                _alliancePhaseDurationInSeconds, 
                _readyTransitionDurationInSeconds);
        }
    }
}
