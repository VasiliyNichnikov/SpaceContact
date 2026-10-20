using System;
using System.Collections.Generic;
using Client.Data.Game;
using Client.Game.Factory;
using Client.Game.Planets;
using Client.Game.Planets.ViewModels;
using Client.UI.HUDs;
using Client.UI.HUDs.ViewModels;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.Game.Field
{
    public sealed class GameFieldPlanetsViewProvider : IDisposable
    {
        private const string PlayerPlanetsRootName = "PlayerPlanetsRoot";
        private const string CenterPlanetsRootName = "CenterPlanetsRoot";
        private const string LeftPlanetsRootName = "LeftPlanetsRoot";
        private const string RightPlanetsRootName = "RightPlanetsRoot";

        private readonly PlayerPlanetsFactory _factory;
        private readonly PlanetLayoutSetData _planetLayoutSetData;
        private readonly IGameCurrentPlayerInfoTabController _currentPlayerInfoTabController;
        private readonly List<PlanetView> _centerOpponentPlanets = new();
        private readonly List<PlanetView> _leftOpponentPlanets = new();
        private readonly List<PlanetView> _rightOpponentPlanets = new();
        private readonly List<PlanetView> _playerPlanets = new();
        private Transform? _playerRootPlanets;
        private Transform? _centerOpponentRootPlanets;
        private Transform? _leftOpponentRootPlanets;
        private Transform? _rightOpponentRootPlanets;
        private bool _isInitialized;

        public GameFieldPlanetsViewProvider(PlayerPlanetsFactory factory, PlanetLayoutSetData planetLayoutSetData,
            IGameCurrentPlayerInfoTabController currentPlayerInfoTabController)
        {
            _factory = factory;
            _planetLayoutSetData = planetLayoutSetData;
            _currentPlayerInfoTabController = currentPlayerInfoTabController;
        }

        public Transform? CenterOpponentRootPlanets => _centerOpponentRootPlanets;

        public Transform? LeftOpponentRootPlanets => _leftOpponentRootPlanets;

        public Transform? RightOpponentRootPlanets => _rightOpponentRootPlanets;

        public IReadOnlyCollection<PlanetView> ViewedOpponentPlanets => _centerOpponentPlanets;

        public IReadOnlyCollection<PlanetView> PlayerPlanets => _playerPlanets;

        public float DistanceBetweenCentralPlanetsByX => _planetLayoutSetData.DistanceBetweenCentralPlanetsByX;

        public void CreateFieldPlanets()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            _currentPlayerInfoTabController.Changed += RefreshCurrentPlayerInfoTab;
        }

        public void InitLeftOpponentPlanets(IReadOnlyList<PlanetViewModel> viewModels)
        {
            InitPlanets(_leftOpponentPlanets, viewModels, ref _leftOpponentRootPlanets,
                LeftPlanetsRootName, -DistanceBetweenCentralPlanetsByX, false);
        }

        public void InitRightOpponentPlanets(IReadOnlyList<PlanetViewModel> viewModels)
        {
            InitPlanets(_rightOpponentPlanets, viewModels, ref _rightOpponentRootPlanets,
                RightPlanetsRootName, DistanceBetweenCentralPlanetsByX, false);
        }

        public void InitCenterOpponentPlanets(IReadOnlyList<PlanetViewModel> viewModels)
        {
            InitPlanets(_centerOpponentPlanets, viewModels, ref _centerOpponentRootPlanets,
                CenterPlanetsRootName, 0.0f, false);
        }

        public void InitPlayerPlanets(IReadOnlyList<PlanetViewModel> viewModels)
        {
            InitPlanets(_playerPlanets, viewModels, ref _playerRootPlanets, PlayerPlanetsRootName, 0.0f, true);
            RefreshCurrentPlayerInfoTab();
        }

        public void ClearSidePlanets()
        {
            DestroyRoot(ref _leftOpponentRootPlanets, _leftOpponentPlanets);
            DestroyRoot(ref _rightOpponentRootPlanets, _rightOpponentPlanets);
        }

        public void Dispose()
        {
            _isInitialized = false;
            _currentPlayerInfoTabController.Changed -= RefreshCurrentPlayerInfoTab;
            ClearSidePlanets();
            DestroyRoot(ref _playerRootPlanets, _playerPlanets);
            DestroyRoot(ref _centerOpponentRootPlanets, _centerOpponentPlanets);
        }

        private static void DestroyRoot(ref Transform? root, List<PlanetView> views)
        {
            if (root != null)
            {
                root.gameObject.SetActive(false);
                UnityEngine.Object.Destroy(root.gameObject);
                root = null;
            }

            views.Clear();
        }

        private void RefreshCurrentPlayerInfoTab()
        {
            foreach (var planetView in _playerPlanets)
            {
                if (_currentPlayerInfoTabController.ActiveTab == GamePlayerInfoTabType.PlanetsDisplay)
                {
                    planetView.Show();
                }
                else
                {
                    planetView.Hide();
                }
            }
        }

        private void InitPlanets(List<PlanetView> views, IReadOnlyList<PlanetViewModel> viewModels,
            ref Transform? root, string rootName, float positionX, bool isPlayer)
        {
            if (views.Count != viewModels.Count)
            {
                DestroyRoot(ref root, views);
            }

            if (viewModels.Count == 0)
            {
                return;
            }

            if (root == null)
            {
                if (!_planetLayoutSetData.TryGetPlanetsLayout(viewModels.Count, isPlayer, out var layout) ||
                    layout.PlanetPositions.Length != viewModels.Count)
                {
                    Logger.Error($"Planet layout does not match the number of planets: {viewModels.Count}.");
                    return;
                }

                root = new GameObject(rootName).transform;
                root.position = new Vector3(positionX, 0.0f, 0.0f);
                foreach (var position in layout.PlanetPositions)
                {
                    views.Add(_factory.CreatePlanet(position, root));
                }
            }

            for (var index = 0; index < views.Count; index++)
            {
                views[index].Init(viewModels[index]);
            }
        }
    }
}
