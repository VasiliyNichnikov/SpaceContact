using System.Collections.Generic;
using System.Linq;
using Core.Game.Players;
using Reactivity;

namespace Client.Game.Planets.ViewModels
{
    public sealed class GameShipsOnPlanetInfoViewModel
    {
        private readonly int _planetId;
        private readonly IGamePlayer _player;
        private readonly ReactivityListProperty<IGameShipsOnPlanetInfoItemViewModel> _infoViewModels = new();
        
        public GameShipsOnPlanetInfoViewModel(
            int planetId, 
            IGamePlayer player)
        {
            _planetId = planetId;
            _player = player;
            _infoViewModels.Value = CreateItemViewModels();
        }

        public IReactivityReadOnlyCollectionProperty<IGameShipsOnPlanetInfoItemViewModel> InfoViewModels =>
            _infoViewModels;
        
        private IReadOnlyCollection<IGameShipsOnPlanetInfoItemViewModel> CreateItemViewModels()
        {
            var result = new List<IGameShipsOnPlanetInfoItemViewModel>();
            var planets = _player.Planets;
            var selectedPlanet = planets.First(p => p.Id == _planetId);
            
            var shipsInfoViewModel = new GameShipsInfoViewModel(_player.Color, selectedPlanet.Ships.Count);
            
            // Порядок добавления важен
            result.Add(shipsInfoViewModel);

            return result;
        }
    }
}