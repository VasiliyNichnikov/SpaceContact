using System;
using System.Collections.Generic;
using Client.Game.Factory;
using Client.Game.Field;
using Client.Game.Planets;
using Client.Game.Planets.ViewModels;
using Client.UI.Utils;
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
        
        public GamePlanetInfoPresenter(
            IGameFieldViewManager fieldViewManager,
            GameFieldPlanetsViewProvider planetsViewProvider,
            GameShipsOnPlanetInfoViewFactory planetInfoViewFactory,
            SceneStorage sceneStorage,
            Camera mainCamera)
        {
            _fieldViewManager = fieldViewManager;
            _planetsViewProvider = planetsViewProvider;
            _planetInfoViewFactory = planetInfoViewFactory;
            _sceneStorage = sceneStorage;
            _fieldViewManager.OnMovementAnimationStarted += OnAnimationMovementStarted;
            _fieldViewManager.OnMovementAnimationEnded += OnAnimationMovementEnded;
            _fieldViewManager.OnInitialized += OnAnimationMovementEnded;
            _mainCamera = mainCamera;
        }

        public void Dispose()
        {
            _fieldViewManager.OnMovementAnimationStarted -= OnAnimationMovementStarted;
            _fieldViewManager.OnMovementAnimationEnded -= OnAnimationMovementEnded;
            _fieldViewManager.OnInitialized -= OnAnimationMovementEnded;
        }

        private void OnAnimationMovementStarted()
        {
            HideUsedPlanetInfoViews();
        }

        private void OnAnimationMovementEnded()
        {
            var opponentGamePlayer = _fieldViewManager.ViewedOpponentPlayer;

            if (opponentGamePlayer == null)
            {
                Logger.Error($"{nameof(GamePlanetInfoPresenter)}.{nameof(OnAnimationMovementEnded)} opponentGamePlayer is null.");
                return;
            }
            
            foreach (var planetView in _planetsViewProvider.ViewedOpponentPlanets)
            {
                var planetInfoView = GetOrCreateInfoView();
                var anchoredPosition = UIUtils.GetPositionOfObjectFromSceneInUI(
                    _mainCamera, 
                    _sceneStorage.MainCanvasRectTransform,
                    planetView.transform.position);
                var viewModel = new GameShipsOnPlanetInfoViewModel(
                    planetView.PlanetId, 
                    opponentGamePlayer);
                planetInfoView.Init(viewModel);
                planetInfoView.RectTransform.anchoredPosition = anchoredPosition;
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