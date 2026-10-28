using System;
using System.Linq;
using System.Collections.Generic;
using Client.UI.HUDs;
using Client.UI.HUDs.ViewModels;
using Core.Game;
using Core.Game.Actions;
using CoreConvertor;
using Reactivity;
using UnityEngine;

namespace Client.Game.Planets.ViewModels
{
    public sealed class GameShipsOnPlanetInfoViewModel : IDisposable
    {
        private const string DefaultBorderColorHex = "#7EFF00";
        private const string PlanetToAttackBorderColorHex = "#FF2900";

        private readonly ReactivityListProperty<IGameShipsOnPlanetInfoItemViewModel> _infoViewModels = new();
        private readonly ReactivityProperty<bool> _isVisible = new();
        private readonly ReactivityProperty<Color> _borderColor = new();
        private readonly ClientGameContext _context;
        private readonly int _planetId;
        private readonly IGameCurrentPlayerInfoTabController _infoTabController;
        private readonly GameActionClient _actionClient;
        private readonly PendingGameActions _pendingActions;
        private bool _isDisposed;

        public GameShipsOnPlanetInfoViewModel(int planetId, ClientGameContext context,
            IGameCurrentPlayerInfoTabController infoTabController, GameActionClient actionClient,
            PendingGameActions pendingActions)
        {
            _planetId = planetId;
            _context = context;
            _infoTabController = infoTabController;
            _actionClient = actionClient;
            _pendingActions = pendingActions;
        }

        public IReactivityReadOnlyCollectionProperty<IGameShipsOnPlanetInfoItemViewModel> InfoViewModels => _infoViewModels;

        public IReactivityProperty<bool> IsVisible => _isVisible;

        public IReactivityProperty<Color> BorderColor => _borderColor;

        public void Init()
        {
            _context.StateChanged += Refresh;
            _pendingActions.Changed += Refresh;
            _infoTabController.Changed += Refresh;
            Refresh();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            _context.StateChanged -= Refresh;
            _pendingActions.Changed -= Refresh;
            _infoTabController.Changed -= Refresh;
            _isDisposed = true;
        }

        private void Refresh()
        {
            var items = new List<IGameShipsOnPlanetInfoItemViewModel>();
            var localPlayer = _context.LocalPlayerState;
            if (localPlayer is null || _context.Galaxy.Planets.All(planet => planet.Id != _planetId))
            {
                _isVisible.Value = false;
                _infoViewModels.Value = items;
                return;
            }

            var ownerId = _context.Galaxy.GetPlanetOwnerId(_planetId);
            _isVisible.Value = ownerId != localPlayer.Id ||
                               _infoTabController.ActiveTab == GamePlayerInfoTabType.PlanetsDisplay;
            var colorHex = _context.Encounter.PlanetIdToAttack == _planetId
                ? PlanetToAttackBorderColorHex
                : DefaultBorderColorHex;
            _borderColor.Value = ColorConvertor.FromCoreColor(Core.EngineData.Color.FromHex(colorHex));

            foreach (var player in _context.Players.All.OrderBy(player => player.Order))
            {
                var count = _context.Galaxy.GetPlayerShipCountOnPlanet(player.Id, _planetId);
                if (count > 0)
                {
                    items.Add(new GameShipsInfoViewModel(player.Color, count));
                }

                var viewModel = new GameChoicePlanetToAttackViewModel(_planetId, player, _context, _actionClient);
                
                if (viewModel.IsButtonVisible())
                {
                    items.Add(viewModel);
                }
            }

            _infoViewModels.Value = items;
        }
    }
}
