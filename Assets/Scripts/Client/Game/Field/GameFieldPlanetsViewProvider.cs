using System.Collections.Generic;
using Client.Data.Game;
using Client.Game.Factory;
using Client.Game.Planets;
using Client.Game.Planets.ViewModels;
using Core.Game;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.Game.Field
{
    public sealed class GameFieldPlanetsViewProvider
    {
        private enum PositionPlanetsType
        {
            Center,
            Left,
            Right
        }
        
        private const string InvalidPlanetsRootName = "InvalidPlanetsRoot";
        private const string LeftPlanetsRootName = "LeftPlanetsRoot";
        private const string RightPlanetsRootName = "RightPlanetsRoot";
        private const string CenterPlanetsRootName = "CenterPlanetsRoot";
        
        private readonly PlayerPlanetsFactory _factory;
        private readonly IGameFieldManager _fieldManager;
        private readonly PlanetLayoutSetData _planetLayoutSetData;
        
        private Transform? _centerOpponentRootPlanets;
        private readonly List<PlanetView> _centerOpponentPlanets = new();
        
        private Transform? _leftOpponentRootPlanets;
        private readonly List<PlanetView> _leftOpponentPlanets = new();
        
        private Transform? _rightOpponentRootPlanets;
        private readonly List<PlanetView> _rightOpponentPlanets = new();
        
        private readonly List<PlanetView> _playerPlanets = new();
        
        public GameFieldPlanetsViewProvider(
            PlayerPlanetsFactory factory,
            IGameFieldManager fieldManager,
            PlanetLayoutSetData planetLayoutSetData)
        {
            _factory = factory;
            _fieldManager = fieldManager;
            _planetLayoutSetData = planetLayoutSetData;
        }
        
        public Transform? CenterOpponentRootPlanets => 
            _centerOpponentRootPlanets;
        
        public Transform? LeftOpponentRootPlanets => 
            _leftOpponentRootPlanets;
        
        public Transform? RightOpponentRootPlanets =>
            _rightOpponentRootPlanets;
        
        public float DistanceBetweenCentralPlanetsByX => 
            _planetLayoutSetData.DistanceBetweenCentralPlanetsByX;
        
        public void CreateFieldPlanets()
        {
            var numberOfPlanetsOnPlayer = _fieldManager.NumberOfPlanetsOnPlayer;
            
            InitCenterPlayerPlanets(numberOfPlanetsOnPlayer);
            InitCenterOpponentPlanets(numberOfPlanetsOnPlayer);
            InitLeftEmptyOpponentPlanets(numberOfPlanetsOnPlayer);
            InitRightEmptyOpponentPlanets(numberOfPlanetsOnPlayer);
        }

        public void InitLeftOpponentPlanets(IReadOnlyList<PlanetViewModel> viewModels)
        {
            InitPlanets(_leftOpponentPlanets, viewModels);
        }

        public void InitRightOpponentPlanets(IReadOnlyList<PlanetViewModel> viewModels)
        {
            InitPlanets(_rightOpponentPlanets, viewModels);
        }

        public void InitCenterOpponentPlanets(IReadOnlyList<PlanetViewModel> viewModels)
        {
            InitPlanets(_centerOpponentPlanets, viewModels);
        }

        public void InitPlayerPlanets(IReadOnlyList<PlanetViewModel> viewModels)
        {
            InitPlanets(_playerPlanets, viewModels);
        }

        private void InitPlanets(IReadOnlyList<PlanetView> views, IReadOnlyList<PlanetViewModel> viewModels)
        {
            if (views.Count != viewModels.Count)
            {
                Logger.Error($"{nameof(GameFieldPlanetsViewProvider)}.{nameof(InitPlanets)}: views.Count != viewModels.Count");
                return;
            }

            for (var i = 0; i < views.Count; i++)
            {
                var view = views[i];
                var viewModel = viewModels[i];
                view.Init(viewModel);
            }
        }

        private void InitCenterPlayerPlanets(int numberOfPlanets)
        {
            InitEmptyPlanets(numberOfPlanets, PositionPlanetsType.Center, 
                _playerPlanets, 
                out _, 
                true);
        }

        private void InitCenterOpponentPlanets(int numberOfPlanets)
        {
            InitEmptyPlanets(numberOfPlanets, PositionPlanetsType.Center, 
                _centerOpponentPlanets, 
                out _centerOpponentRootPlanets,
                false);
        }

        private void InitLeftEmptyOpponentPlanets(int numberOfPlanets)
        {
            InitEmptyPlanets(numberOfPlanets, PositionPlanetsType.Left, 
                _leftOpponentPlanets, 
                out _leftOpponentRootPlanets,
                false);
        }

        private void InitRightEmptyOpponentPlanets(int numberOfPlanets)
        {
            InitEmptyPlanets(numberOfPlanets, PositionPlanetsType.Right,
                _rightOpponentPlanets,
                out _rightOpponentRootPlanets,
                false);
        }
        
        private void InitEmptyPlanets(
            int numberOfPlanets, 
            PositionPlanetsType positionPlanetsType,
            List<PlanetView> createdPlanets,
            out Transform rootPlanetsTransform,
            bool isPlayer)
        {
            var rootPlanetsName = GetRootPlanetsNameByType(positionPlanetsType);
            var rootPlanets = new GameObject(rootPlanetsName)
            {
                transform =
                {
                    position = Vector3.zero
                }
            };
            
            rootPlanetsTransform = rootPlanets.transform;
            var layout = isPlayer 
                ? _planetLayoutSetData.GetPlayerPlanetsLayoutData(numberOfPlanets)
                : _planetLayoutSetData.GetOppositePlanetsLayoutData(numberOfPlanets);

            if (layout.PlanetPositions.Length != numberOfPlanets)
            {
                Logger.Error($"{nameof(GameFieldPlanetsViewProvider)}.{nameof(InitEmptyPlanets)}: numberOfPlanets != layout planets length.");
                return;
            }
            
            for (var i = 0; i < numberOfPlanets; i++)
            {
                var localPlanetPosition = layout.PlanetPositions[i];
                
                var createdPlanet = _factory.CreatePlanet(localPlanetPosition, rootPlanets.transform);
                createdPlanets.Add(createdPlanet);
            }
            
            var currentRootPosition = rootPlanets.transform.position;
            var refreshedRootXPosition = GetRootPositionByX(currentRootPosition.x, positionPlanetsType);
            
            var refreshedRootPosition = new Vector3(
                refreshedRootXPosition,
                currentRootPosition.y,
                currentRootPosition.z);
            rootPlanets.transform.position = refreshedRootPosition;
        }

        private static string GetRootPlanetsNameByType(PositionPlanetsType positionType)
        {
            switch (positionType)
            {
                case PositionPlanetsType.Center:
                    return CenterPlanetsRootName;
                
                case PositionPlanetsType.Left:
                    return LeftPlanetsRootName;
                
                case PositionPlanetsType.Right:
                    return RightPlanetsRootName;
                
                default:
                    Logger.Error($"{nameof(GameFieldPlanetsViewProvider)}.{nameof(GetRootPlanetsNameByType)}: planetsType is not supported: {positionType}.");
                    return InvalidPlanetsRootName;
            }
        }

        private float GetRootPositionByX(float currentPositionX, PositionPlanetsType positionType)
        {
            switch (positionType)
            {
                case PositionPlanetsType.Center:
                    return currentPositionX;
                    
                case PositionPlanetsType.Left:
                    return currentPositionX - _planetLayoutSetData.DistanceBetweenCentralPlanetsByX;
                
                case PositionPlanetsType.Right:
                    return currentPositionX + _planetLayoutSetData.DistanceBetweenCentralPlanetsByX;
                
                default:
                    Logger.Error($"{nameof(GameFieldPlanetsViewProvider)}.{nameof(GetRootPlanetsNameByType)}: planetsType is not supported: {positionType}.");
                    return currentPositionX;
            }
        }
    }
}