using System;
using System.Collections.Generic;
using Client.Game.Factory;
using Client.Game.Field;
using Client.Game.Planets;
using Client.Game.Planets.ViewModels;
using Client.UI.Utils;
using Core.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Client.UI
{
    public sealed class GamePlanetInfoPresenter : IDisposable
    {
        private readonly Queue<GameShipsOnPlanetInfoView> _usedPlanetInfoViews = new();
        private readonly Queue<GameShipsOnPlanetInfoView> _unusedPlanetInfoViews = new();

        private readonly GameContext _context;
        private readonly IGameFieldViewManager _fieldViewManager;
        private readonly GameFieldPlanetsViewProvider _planetsViewProvider;
        private readonly GameShipsOnPlanetInfoViewFactory _planetInfoViewFactory;
        private readonly Camera _mainCamera;
        private readonly SceneStorage _sceneStorage;
        private readonly GameShipsOnPlanetInfoViewModelFactory _shipsInfoViewModelFactory;

        private readonly List<GameShipsOnPlanetInfoViewModel> _opponentPlanetInfoViewModels = new();
        private readonly List<GameShipsOnPlanetInfoViewModel> _currentPlayerPlanetInfoViewModels = new();

        public GamePlanetInfoPresenter(
            GameContext context,
            IGameFieldViewManager fieldViewManager,
            GameFieldPlanetsViewProvider planetsViewProvider,
            GameShipsOnPlanetInfoViewFactory planetInfoViewFactory,
            SceneStorage sceneStorage,
            Camera mainCamera,
            GameShipsOnPlanetInfoViewModelFactory shipsInfoViewModelFactory)
        {
            _context = context;
            _fieldViewManager = fieldViewManager;
            _planetsViewProvider = planetsViewProvider;
            _planetInfoViewFactory = planetInfoViewFactory;
            _sceneStorage = sceneStorage;
            _mainCamera = mainCamera;
            _shipsInfoViewModelFactory = shipsInfoViewModelFactory;
        }

        public void Init()
        {
            _fieldViewManager.OnMovementAnimationStarted += OnAnimationMovementStarted;
            _fieldViewManager.OnMovementAnimationEnded += DisplayInfoAboutViewedPlanets;
            _fieldViewManager.OnInitialized += DisplayInfoAboutViewedPlanets;
            DisplayInfoAboutViewedPlanets();
        }

        public void Dispose()
        {
            HideUsedPlanetInfoViews();
            _fieldViewManager.OnMovementAnimationStarted -= OnAnimationMovementStarted;
            _fieldViewManager.OnMovementAnimationEnded -= DisplayInfoAboutViewedPlanets;
            _fieldViewManager.OnInitialized -= DisplayInfoAboutViewedPlanets;
            DisposeShipsInfoViewModels(_opponentPlanetInfoViewModels);
            DisposeShipsInfoViewModels(_currentPlayerPlanetInfoViewModels);
            
            while (_unusedPlanetInfoViews.Count > 0)
            {
                Object.Destroy(_unusedPlanetInfoViews.Dequeue().gameObject);
            }
        }

        private static void DisposeShipsInfoViewModels(List<GameShipsOnPlanetInfoViewModel> viewModels)
        {
            if (viewModels.Count == 0)
            {
                return;
            }

            foreach (var viewModel in viewModels)
            {
                viewModel.Dispose();
            }

            viewModels.Clear();
        }

        private void OnAnimationMovementStarted()
        {
            HideUsedPlanetInfoViews();
            DisposeShipsInfoViewModels(_opponentPlanetInfoViewModels);
            DisposeShipsInfoViewModels(_currentPlayerPlanetInfoViewModels);
        }

        private void DisplayInfoAboutViewedPlanets()
        {
            HideUsedPlanetInfoViews();
            DisposeShipsInfoViewModels(_opponentPlanetInfoViewModels);
            DisposeShipsInfoViewModels(_currentPlayerPlanetInfoViewModels);
            DisplayInfoAboutCurrentPlayerPlanets();
            DisplayInfoAboutOpponentPlanets();
        }

        private void DisplayInfoAboutOpponentPlanets()
        {
            var opponentGamePlayer = _fieldViewManager.ViewedOpponentPlayer;

            if (opponentGamePlayer == null)
            {
                return;
            }

            DisplayInfoAboutSelectedPlayerPlanets(
                _planetsViewProvider.ViewedOpponentPlanets,
                _opponentPlanetInfoViewModels);
        }

        private void DisplayInfoAboutCurrentPlayerPlanets()
        {
            var localPlayer = _context.LocalPlayerState;

            if (localPlayer == null)
            {
                return;
            }

            DisplayInfoAboutSelectedPlayerPlanets(
                _planetsViewProvider.PlayerPlanets,
                _currentPlayerPlanetInfoViewModels);
        }

        private void DisplayInfoAboutSelectedPlayerPlanets(
            IReadOnlyCollection<PlanetView> planetViews,
            List<GameShipsOnPlanetInfoViewModel> storagePlanetViewModels)
        {
            var localPlayer = _context.LocalPlayerState;

            if (localPlayer == null)
            {
                return;
            }

            DisposeShipsInfoViewModels(storagePlanetViewModels);

            foreach (var planetView in planetViews)
            {
                var planetInfoView = GetOrCreateInfoView();
                var anchoredPosition = UIUtils.GetPositionOfObjectFromSceneInUI(
                    _mainCamera,
                    _sceneStorage.MainCanvasRectTransform,
                    planetView.transform.position);
                var viewModel = _shipsInfoViewModelFactory.Create(planetView.PlanetId);
                planetInfoView.Init(viewModel);
                planetInfoView.RectTransform.anchoredPosition = anchoredPosition;
                storagePlanetViewModels.Add(viewModel);
            }
        }

        private void HideUsedPlanetInfoViews()
        {
            while (_usedPlanetInfoViews.Count > 0)
            {
                var infoView = _usedPlanetInfoViews.Dequeue();
                infoView.Hide();
                _unusedPlanetInfoViews.Enqueue(infoView);
            }
        }

        private GameShipsOnPlanetInfoView GetOrCreateInfoView()
        {
            if (_unusedPlanetInfoViews.Count > 0)
            {
                var infoView = _unusedPlanetInfoViews.Dequeue();
                infoView.Show();
                _usedPlanetInfoViews.Enqueue(infoView);
                return infoView;
            }

            var createdInfoView = _planetInfoViewFactory.Create();
            _usedPlanetInfoViews.Enqueue(createdInfoView);
            return createdInfoView;
        }
    }
}
