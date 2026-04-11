using Core.Game.Planets;
using GeneralUtils;

namespace Client.Game.Planets.ViewModels
{
    public sealed class GameChoicePlanetToAttackViewModel : IGameShipsOnPlanetInfoItemViewModel
    {
        private readonly int _planetId;
        private readonly GamePlanetAttackTargetSelector _attackTargetSelector;
        
        public GameChoicePlanetToAttackViewModel(
            int planetId, 
            GamePlanetAttackTargetSelector attackTargetSelector)
        {
            _planetId = planetId;
            _attackTargetSelector = attackTargetSelector;
        }

        public void OnAttackPlanetButtonClickHandler()
        {
            if (_attackTargetSelector.IsWaitingServer)
            {
                return;
            }
            
            _attackTargetSelector
                .SelectTargetAsync(_planetId)
                .FireAndForget();
        }

        public void Apply(IGameShipsOnPlanetInfoVisitor visitor) => 
            visitor.Visit(this);
    }
}