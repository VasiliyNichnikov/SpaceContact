#if UNITY_EDITOR
using System;
using Client.Configs.Game;

namespace Editor
{
    public sealed class PlanetsConfigValidation : IGameRulesValidation
    {
        public void Validate(GameRulesValidationContext context)
        {
            context.ValidateAssets<RulesOfPlanetsConfig>(config => ValidatePlanets(config, context.MaximumPlayerCount));
        }

        private static void ValidatePlanets(RulesOfPlanetsConfig config, int maximumPlayerCount)
        {
            var rules = config.BuildData();

            if (rules.NumberOfPlanetsPlayer <= 0 || rules.NumberOfShipsOnPlanet <= 0)
            {
                throw new InvalidOperationException("Each player must start with planets and ships on every planet.");
            }

            var planetCount = (long)maximumPlayerCount * rules.NumberOfPlanetsPlayer;

            if (planetCount > int.MaxValue || planetCount * rules.NumberOfShipsOnPlanet > int.MaxValue)
            {
                throw new InvalidOperationException("Initial planet or ship count exceeds the supported ID range.");
            }
        }
    }
}
#endif
