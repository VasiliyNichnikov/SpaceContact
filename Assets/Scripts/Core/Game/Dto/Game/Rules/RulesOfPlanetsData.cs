namespace Core.Game.Dto.Game.Rules
{
    public readonly struct RulesOfPlanetsData
    {
        public readonly int NumberOfPlanetsPlayer;

        public readonly int NumberOfShipsOnPlanet;

        public RulesOfPlanetsData(int numberOfPlanetsPlayer, int numberOfShipsOnPlanet)
        {
            NumberOfPlanetsPlayer = numberOfPlanetsPlayer;
            NumberOfShipsOnPlanet = numberOfShipsOnPlanet;
        }
    }
}