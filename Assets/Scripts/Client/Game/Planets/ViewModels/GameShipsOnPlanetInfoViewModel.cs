using System;
using System.Collections.Generic;
using System.Linq;
using Client.UI.HUDs;
using Client.UI.HUDs.ViewModels;
using Core.Game.Encounter;
using Core.Game.Phases.Client;
using Core.Game.Planets;
using Core.Game.Players;
using Core.Game.Rules;
using Reactivity;

namespace Client.Game.Planets.ViewModels
{
    public sealed class GameShipsOnPlanetInfoViewModel : IDisposable
    {
        private readonly ReactivityListProperty<IGameShipsOnPlanetInfoItemViewModel> _infoViewModels = new();
        private readonly ReactivityProperty<bool> _isVisible = new();
        
        private readonly int _planetId;
        private readonly ulong _ownerClientPlayerId;
        private readonly IGamePlayer _planetPlayerOwner;
        private readonly GameRulesChecker _rulesChecker;
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly IGameClientEncounterManager _encounterManager;
        private readonly GamePlanetAttackTargetSelector _attackTargetSelector;
        private readonly IGameCurrentPlayerInfoTabController _infoTabController;
        
        public GameShipsOnPlanetInfoViewModel(
            int planetId,
            ulong ownerClientPlayerId,
            IGamePlayer planetPlayerOwner,
            GameRulesChecker rulesChecker,
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            IGameClientEncounterManager encounterManager,
            GamePlanetAttackTargetSelector attackTargetSelector,
            IGameCurrentPlayerInfoTabController infoTabController)
        {
            _planetId = planetId;
            _ownerClientPlayerId = ownerClientPlayerId;
            _planetPlayerOwner = planetPlayerOwner;
            _rulesChecker = rulesChecker;
            _destinyPhaseResolver = destinyPhaseResolver;
            _encounterManager = encounterManager;
            _attackTargetSelector = attackTargetSelector;
            _infoTabController = infoTabController;
            
            _destinyPhaseResolver.Changed += RefreshInfoViewModels;
            _encounterManager.DefenderChanged += RefreshInfoViewModels;
            _encounterManager.PlanetChanged += RefreshInfoViewModels;
            _infoTabController.Changed += RefreshPlanetInfoVisibility;
            RefreshInfoViewModels();
        }

        public IReactivityReadOnlyCollectionProperty<IGameShipsOnPlanetInfoItemViewModel> InfoViewModels =>
            _infoViewModels;

        public IReactivityProperty<bool> IsVisible => 
            _isVisible;

        public void Dispose()
        {
            _destinyPhaseResolver.Changed -= RefreshInfoViewModels;
            _encounterManager.DefenderChanged -= RefreshInfoViewModels;
            _encounterManager.PlanetChanged -= RefreshInfoViewModels;
            _infoTabController.Changed -= RefreshPlanetInfoVisibility;
        }

        private void RefreshInfoViewModels()
        {
            RefreshPlanetInfoVisibility();
            _infoViewModels.Value = CreateItemViewModels();
        }
        
        private IReadOnlyCollection<IGameShipsOnPlanetInfoItemViewModel> CreateItemViewModels()
        {
            var result = new List<IGameShipsOnPlanetInfoItemViewModel>();
            
            // Порядок добавления важен
            AddShipsInfoViewModel(result);
            TryAddChoicePlanetToAttackViewModel(result);
            
            return result;
        }

        private void AddShipsInfoViewModel(List<IGameShipsOnPlanetInfoItemViewModel> items)
        {
            var planets = _planetPlayerOwner.Planets;
            var selectedPlanet = planets.First(p => p.Id == _planetId);
            var shipsInfoViewModel = new GameShipsInfoViewModel(_planetPlayerOwner.Color, selectedPlanet.Ships.Count);
            
            items.Add(shipsInfoViewModel);
        }

        private void TryAddChoicePlanetToAttackViewModel(List<IGameShipsOnPlanetInfoItemViewModel> items)
        {
            var context = GameRuleContext.CheckPlanet(_ownerClientPlayerId, _planetId);
            
            if (!_rulesChecker.Check(GameRuleType.CanAggressorAttackToPlanet, context))
            {
                return;
            }

            var viewModel = new GameChoicePlanetToAttackViewModel(_planetId, _attackTargetSelector);
            items.Add(viewModel);
        }

        private void RefreshPlanetInfoVisibility()
        {
            var defaultVisibleCondition = _encounterManager.PlanetIdToAttack == null ||
                                          _encounterManager.PlanetIdToAttack == _planetId;
            
            if (_planetPlayerOwner.PlayerId == _ownerClientPlayerId)
            {
                _isVisible.Value = _infoTabController.ActiveTab == GamePlayerInfoTabType.PlanetsDisplay &&
                                   defaultVisibleCondition;
            }
            else
            {
                _isVisible.Value = defaultVisibleCondition;
            }
        }
    }
}