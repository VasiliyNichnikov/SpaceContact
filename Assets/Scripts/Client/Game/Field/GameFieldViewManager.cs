using System;
using System.Collections.Generic;
using System.Linq;
using Client.Game.Planets.ViewModels;
using Core.Game;
using DG.Tweening;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.Game.Field
{
    public sealed class GameFieldViewManager : IGameFieldViewManager, IDisposable
    {
        private const float AnimationDurationBetweenPlanets = 0.7f;

        private readonly GameFieldPlanetsViewProvider _planetsViewProvider;
        private readonly GameContext _gameContext;

        private readonly List<GamePlayerData> _opponents = new();

        private IReadOnlyList<GamePlayerData> _displayedPlayers = Array.Empty<GamePlayerData>();
        private IReadOnlyList<PlanetData> _displayedPlanets = Array.Empty<PlanetData>();
        private int _selectedOpponentIndex;
        private Sequence? _currentSequenceAnimation;

        public GameFieldViewManager(GameFieldPlanetsViewProvider planetsViewProvider, GameContext gameContext)
        {
            _planetsViewProvider = planetsViewProvider;
            _gameContext = gameContext;
        }

        public event Action? OnViewedOpponentChanged;

        public event Action? OnInitialized;

        public event Action? OnMovementAnimationStarted;

        public event Action? OnMovementAnimationEnded;

        public GamePlayerData? ViewedOpponentPlayer { get; private set; }

        public void Init()
        {
            _gameContext.StateChanged += RefreshField;
            RefreshField();
        }

        public void Dispose()
        {
            _gameContext.StateChanged -= RefreshField;
            _currentSequenceAnimation?.Kill();
            _displayedPlayers = Array.Empty<GamePlayerData>();
            _displayedPlanets = Array.Empty<PlanetData>();
            _opponents.Clear();
            _selectedOpponentIndex = 0;
            ViewedOpponentPlayer = null;
        }

        public bool CanMoveToLeftOpponent() =>
            _selectedOpponentIndex > 0;

        public bool CanMoveToRightOpponent() =>
            _selectedOpponentIndex < _opponents.Count - 1;

        public void MoveToLeftOpponent()
        {
            if (_currentSequenceAnimation != null || !CanMoveToLeftOpponent())
            {
                return;
            }

            _selectedOpponentIndex--;
            var deltaX = _planetsViewProvider.DistanceBetweenCentralPlanetsByX;
            MoveToOtherOpponent(_planetsViewProvider.InitLeftOpponentPlanets, deltaX, AnimationDurationBetweenPlanets);
        }

        public void MoveToRightOpponent()
        {
            if (_currentSequenceAnimation != null || !CanMoveToRightOpponent())
            {
                return;
            }

            _selectedOpponentIndex++;
            var deltaX = -_planetsViewProvider.DistanceBetweenCentralPlanetsByX;
            MoveToOtherOpponent(_planetsViewProvider.InitRightOpponentPlanets, deltaX, AnimationDurationBetweenPlanets);
        }

        private static void JoinMoveSequence(Sequence sequence, Transform objTransform, float deltaX, float duration)
        {
            var targetPosition = objTransform.position;
            targetPosition.x += deltaX;
            sequence.Join(objTransform
                .DOMove(targetPosition, duration)
                .SetEase(Ease.InOutSine));
        }

        private void RefreshField()
        {
            if (_gameContext.LocalPlayerState is not { } localPlayer)
            {
                return;
            }

            var players = _gameContext.Players.All.OrderBy(player => player.Order).ToList();
            var planets = _gameContext.Galaxy.Planets.ToList();
            
            if (_displayedPlayers.SequenceEqual(players) && _displayedPlanets.SequenceEqual(planets))
            {
                return;
            }

            _displayedPlayers = players;
            _displayedPlanets = planets;
            var viewedPlayerId = ViewedOpponentPlayer?.Id;
            _currentSequenceAnimation?.Kill();
            _planetsViewProvider.ClearSidePlanets();
            _planetsViewProvider.InitPlayerPlanets(CreatePlanetsViewModelsForPlayer(localPlayer));
            _opponents.Clear();
            _opponents.AddRange(players.Where(player => player.Id != localPlayer.Id));
            _selectedOpponentIndex = _opponents.FindIndex(player => player.Id == viewedPlayerId);
            
            if (_selectedOpponentIndex < 0)
            {
                _selectedOpponentIndex = 0;
            }

            ViewedOpponentPlayer = _opponents.Count > 0 ? _opponents[_selectedOpponentIndex] : null;
            var viewModels = ViewedOpponentPlayer is { } opponent
                ? CreatePlanetsViewModelsForPlayer(opponent)
                : Array.Empty<PlanetViewModel>();
            ResetPositionsAndSetCenterOpponentPlanets(viewModels);
            OnViewedOpponentChanged?.Invoke();
            OnInitialized?.Invoke();
        }

        private void MoveToOtherOpponent(
            Action<IReadOnlyList<PlanetViewModel>> initOpponentPlanetsAction,
            float deltaX,
            float duration)
        {
            var otherOpponent = _opponents[_selectedOpponentIndex];
            var otherOpponentViewModels = CreatePlanetsViewModelsForPlayer(otherOpponent);
            initOpponentPlanetsAction.Invoke(otherOpponentViewModels);
            MoveOpponentPlanetsOnAxisX(deltaX, duration, () => ResetPositionsAndSetCenterOpponentPlanets(otherOpponentViewModels));

            ViewedOpponentPlayer = otherOpponent;
            OnViewedOpponentChanged?.Invoke();
        }

        private void MoveOpponentPlanetsOnAxisX(float deltaX, float duration, Action? onCompleteAction)
        {
            if (_currentSequenceAnimation != null)
            {
                Logger.Error("GameFieldViewManager: MoveOpponentPlanetsOnAxisX animation is already running.");
                return;
            }

            var sequence = DOTween.Sequence();
            var provider = _planetsViewProvider;

            if (provider.LeftOpponentRootPlanets != null)
            {
                JoinMoveSequence(sequence, provider.LeftOpponentRootPlanets, deltaX, duration);
            }

            if (provider.CenterOpponentRootPlanets != null)
            {
                JoinMoveSequence(sequence, provider.CenterOpponentRootPlanets, deltaX, duration);
            }

            if (provider.RightOpponentRootPlanets != null)
            {
                JoinMoveSequence(sequence, provider.RightOpponentRootPlanets, deltaX, duration);
            }

            sequence.OnComplete(() =>
            {
                onCompleteAction?.Invoke();
                _currentSequenceAnimation = null;
                OnMovementAnimationEnded?.Invoke();
            });

            sequence.OnKill(() => _currentSequenceAnimation = null);

            _currentSequenceAnimation = sequence;
            OnMovementAnimationStarted?.Invoke();
            sequence.Play();
        }

        private void ResetPositionsAndSetCenterOpponentPlanets(IReadOnlyList<PlanetViewModel> viewModels)
        {
            var provider = _planetsViewProvider;

            if (provider.LeftOpponentRootPlanets != null)
            {
                provider.LeftOpponentRootPlanets.position = new Vector3(-provider.DistanceBetweenCentralPlanetsByX, 0.0f, 0.0f);
            }

            if (provider.RightOpponentRootPlanets != null)
            {
                provider.RightOpponentRootPlanets.position = new Vector3(provider.DistanceBetweenCentralPlanetsByX, 0.0f, 0.0f);
            }

            if (provider.CenterOpponentRootPlanets != null)
            {
                provider.CenterOpponentRootPlanets.position = Vector3.zero;
            }

            provider.InitCenterOpponentPlanets(viewModels);
            provider.ClearSidePlanets();
        }

        private IReadOnlyList<PlanetViewModel> CreatePlanetsViewModelsForPlayer(GamePlayerData player)
        {
            var planets = _gameContext.Galaxy.GetPlanetsByOwner(player.Id);
            var viewModels = planets
                .Select(p => new PlanetViewModel(player, p))
                .ToList();

            return viewModels;
        }
    }
}
