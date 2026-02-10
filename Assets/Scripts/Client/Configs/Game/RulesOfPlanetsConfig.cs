using Core.Game.Dto.Game.Rules;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "RulesOfPlanetsConfig", menuName = "Configs/Game/RulesOfPlanetsConfig", order = 0)]
    public class RulesOfPlanetsConfig : ScriptableObject
    {
        [Header("Кол-во планет у игрока")]
        [SerializeField, Min(4)]
        private int _numberOfPlanetsPlayer;

        [Header("Кол-во кораблей на планете")]
        [SerializeField, Min(1)]
        private int _numberOfShipsOnPlanet;

        public RulesOfPlanetsData BuildData()
        {
            return new RulesOfPlanetsData(_numberOfPlanetsPlayer, _numberOfShipsOnPlanet);
        }
    }
}