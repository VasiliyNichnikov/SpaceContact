using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ReactivityProperty<bool> _isPlanetInfoVisible = new();
        
        private readonly int _planetId;
        private readonly ulong _ownerClientPlayerId;
        private readonly IGamePlayer _planetPlayerOwner;
        private readonly GameRulesChecker _rulesChecker;
        private readonly IGameClientDestinyPhaseResolver _destinyPhaseResolver;
        private readonly IGameClientEncounterManager _encounterManager;
        private readonly GamePlanetAttackTargetSelector _attackTargetSelector;
        
        public GameShipsOnPlanetInfoViewModel(
            int planetId,
            ulong ownerClientPlayerId,
            IGamePlayer planetPlayerOwner,
            GameRulesChecker rulesChecker,
            IGameClientDestinyPhaseResolver destinyPhaseResolver,
            IGameClientEncounterManager encounterManager,
            GamePlanetAttackTargetSelector attackTargetSelector)
        {
            _planetId = planetId;
            _ownerClientPlayerId = ownerClientPlayerId;
            _planetPlayerOwner = planetPlayerOwner;
            _rulesChecker = rulesChecker;
            _destinyPhaseResolver = destinyPhaseResolver;
            _encounterManager = encounterManager;
            _attackTargetSelector = attackTargetSelector;
            
            _destinyPhaseResolver.Changed += RefreshInfoViewModels;
            _encounterManager.DefenderChanged += RefreshInfoViewModels;
            _encounterManager.PlanetChanged += RefreshInfoViewModels;
            RefreshInfoViewModels();
        }

        public IReactivityReadOnlyCollectionProperty<IGameShipsOnPlanetInfoItemViewModel> InfoViewModels =>
            _infoViewModels;

        public IReactivityProperty<bool> IsPlanetInfoVisible => 
            _isPlanetInfoVisible;

        public void Dispose()
        {
            _destinyPhaseResolver.Changed -= RefreshInfoViewModels;
            _encounterManager.DefenderChanged -= RefreshInfoViewModels;
            _encounterManager.PlanetChanged -= RefreshInfoViewModels;
        }

        private void RefreshInfoViewModels()
        {
            _isPlanetInfoVisible.Value = _encounterManager.PlanetIdToAttack == null || 
                                         _encounterManager.PlanetIdToAttack == _planetId;
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
    }
}