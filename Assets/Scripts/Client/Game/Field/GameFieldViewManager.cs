using System;
using System.Collections.Generic;
using System.Linq;
using Client.Game.Planets.ViewModels;
using Core.Game;
using Core.Game.Players;
using DG.Tweening;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.Game.Field
{
    public sealed class GameFieldViewManager : IGameFieldViewManager
    {
        private const float AnimationDurationBetweenPlanets = 0.7f;
        
        private readonly IGameFieldManager _fieldManager;
        private readonly GameFieldPlanetsViewProvider _planetsViewProvider;

        private readonly List<IGamePlayer> _opponents = new();
        
        private int _selectedOpponentIndex;
        private Sequence? _currentSequenceAnimation;
        
        public GameFieldViewManager(
            IGameFieldManager fieldManager, 
            GameFieldPlanetsViewProvider planetsViewProvider)
        {
            _fieldManager = fieldManager;
            _planetsViewProvider = planetsViewProvider;
        }
        
        public void Init()
        {
            var playerPlanetsViewModels = CreatePlanetsViewModelsForPlayer(_fieldManager.CurrentPlayer);
            _planetsViewProvider.InitPlayerPlanets(playerPlanetsViewModels);
            
            _opponents.AddRange(_fieldManager.Opponents);

            if (_opponents.Count == 0)
            {
                Logger.Error($"{nameof(GameFieldViewManager)}.{nameof(Init)}: opponents list is empty.");
                
                return;
            }

            _selectedOpponentIndex = 0;
            var firstOpponent = _opponents[_selectedOpponentIndex];
            var opponentPlanetsViewModels = CreatePlanetsViewModelsForPlayer(firstOpponent);
            _planetsViewProvider.InitCenterOpponentPlanets(opponentPlanetsViewModels);
        }

        public bool CanMoveToLeftOpponent() => 
            _selectedOpponentIndex > 0;

        public bool CanMoveToRightOpponent() => 
            _selectedOpponentIndex < _opponents.Count - 1;
        
        public void MoveToLeftOpponent()
        {
            if (_currentSequenceAnimation != null)
            {
                return;
            }
            
            _selectedOpponentIndex--;
            var deltaX = _planetsViewProvider.DistanceBetweenCentralPlanetsByX;
            MoveToOtherOpponent(_planetsViewProvider.InitLeftOpponentPlanets, deltaX, AnimationDurationBetweenPlanets);
        }
        
        public void MoveToRightOpponent()
        {
            if (_currentSequenceAnimation != null)
            {
                return;
            }
            
            _selectedOpponentIndex++;
            var deltaX = -_planetsViewProvider.DistanceBetweenCentralPlanetsByX;
            MoveToOtherOpponent(_planetsViewProvider.InitRightOpponentPlanets, deltaX, AnimationDurationBetweenPlanets);
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
            });

            sequence.OnKill(() => _currentSequenceAnimation = null);
            
            sequence.Play();
            _currentSequenceAnimation = sequence;
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
        }
        
        private static void JoinMoveSequence(Sequence sequence, Transform objTransform, float deltaX, float duration)
        {
            var targetPosition = objTransform.position;
            targetPosition.x += deltaX;
            sequence.Join(objTransform
                .DOMove(targetPosition, duration)
                .SetEase(Ease.InOutSine));
        } 

        private static IReadOnlyList<PlanetViewModel> CreatePlanetsViewModelsForPlayer(IGamePlayer player)
        {
            var planets = player.Planets;
            var viewModels = planets
                .Select(p => new PlanetViewModel(player, p))
                .ToList();

            return viewModels;
        }
    }
}