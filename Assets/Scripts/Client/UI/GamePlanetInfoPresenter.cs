using System;
using System.Collections.Generic;
using Client.Game.Factory;
using Client.Game.Field;
using Client.Game.Planets;
using Client.Game.Planets.ViewModels;
using Client.UI.Utils;
using Core.Game.Players;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.UI
{
    public sealed class GamePlanetInfoPresenter : IDisposable
    {
        private readonly Queue<GameShipsOnPlanetInfoView> _usedPlanetInfoViews = new();
        private readonly Queue<GameShipsOnPlanetInfoView> _unusedPlanetInfoViews = new();
        
        private readonly IGameFieldViewManager _fieldViewManager;
        private readonly GameFieldPlanetsViewProvider _planetsViewProvider;
        private readonly GameShipsOnPlanetInfoViewFactory _planetInfoViewFactory;
        private readonly Camera _mainCamera;
        private readonly SceneStorage _sceneStorage;
        private readonly GamePlayersRegistry _playersRegistry;
        private readonly GameShipsOnPlanetInfoViewModelFactory _shipsInfoViewModelFactory;
        
        private readonly List<GameShipsOnPlanetInfoViewModel> _opponentPlanetInfoViewModels = new();
        private readonly List<GameShipsOnPlanetInfoViewModel> _currentPlayerPlanetInfoViewModels = new();
        
        public GamePlanetInfoPresenter(
            IGameFieldViewManager fieldViewManager,
            GameFieldPlanetsViewProvider planetsViewProvider,
            GameShipsOnPlanetInfoViewFactory planetInfoViewFactory,
            SceneStorage sceneStorage,
            GamePlayersRegistry playersRegistry,
            Camera mainCamera,
            GameShipsOnPlanetInfoViewModelFactory shipsInfoViewModelFactory)
        {
            _fieldViewManager = fieldViewManager;
            _planetsViewProvider = planetsViewProvider;
            _planetInfoViewFactory = planetInfoViewFactory;
            _sceneStorage = sceneStorage;
            _playersRegistry = playersRegistry;
            _fieldViewManager.OnMovementAnimationStarted += OnAnimationMovementStarted;
            _fieldViewManager.OnMovementAnimationEnded += OnAnimationMovementEnded;
            _fieldViewManager.OnInitialized += DisplayInfoAboutViewedPlanets;
            _mainCamera = mainCamera;
            _shipsInfoViewModelFactory = shipsInfoViewModelFactory;
        }

        public void Dispose()
        {
            _fieldViewManager.OnMovementAnimationStarted -= OnAnimationMovementStarted;
            _fieldViewManager.OnMovementAnimationEnded -= OnAnimationMovementEnded;
            _fieldViewManager.OnInitialized -= DisplayInfoAboutViewedPlanets;
            DisposeShipsInfoViewModels(_opponentPlanetInfoViewModels);
            DisposeShipsInfoViewModels(_currentPlayerPlanetInfoViewModels);
        }

        private void OnAnimationMovementStarted()
        {
            HideUsedPlanetInfoViews();
        }

        private void DisplayInfoAboutViewedPlanets()
        {
            DisplayInfoAboutCurrentPlayerPlanets();
            DisplayInfoAboutOpponentPlanets();
        }
        
        private void OnAnimationMovementEnded() => 
            DisplayInfoAboutOpponentPlanets();
        
        private void DisplayInfoAboutOpponentPlanets()
        {
            var opponentGamePlayer = _fieldViewManager.ViewedOpponentPlayer;

            if (opponentGamePlayer == null)
            {
                Logger.Error($"{nameof(GamePlanetInfoPresenter)}.{nameof(DisplayInfoAboutOpponentPlanets)}: opponentGamePlayer is null.");
                return;
            }

            DisplayInfoAboutSelectedPlayerPlanets(
                opponentGamePlayer,
                _planetsViewProvider.ViewedOpponentPlanets,
                _opponentPlanetInfoViewModels);
        }
        
        private void DisplayInfoAboutCurrentPlayerPlanets()
        {
            var ownerPlayer = _playersRegistry.GetOwnerWithError();

            if (ownerPlayer == null)
            {
                return;
            }
            
            DisplayInfoAboutSelectedPlayerPlanets(
                ownerPlayer, 
                _planetsViewProvider.PlayerPlanets, 
                _currentPlayerPlanetInfoViewModels);
        }
        
        private void DisplayInfoAboutSelectedPlayerPlanets(
            IGamePlayer gamePlayer, 
            IReadOnlyCollection<PlanetView> planetViews,
            List<GameShipsOnPlanetInfoViewModel> storagePlanetViewModels)
        {
            var ownerPlayer = _playersRegistry.GetOwnerWithError();

            if (ownerPlayer == null)
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
                var viewModel = _shipsInfoViewModelFactory.Create(
                    planetView.PlanetId, 
                    ownerPlayer.PlayerId,
                    gamePlayer);
                planetInfoView.Init(viewModel);
                planetInfoView.RectTransform.anchoredPosition = anchoredPosition;
                storagePlanetViewModels.Add(viewModel);
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