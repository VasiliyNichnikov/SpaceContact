using System;
using System.Collections.Generic;
using Logs;

namespace Core.Game.Phases.Server
{
    public sealed class GameServerPhaseTransitioner : IDisposable
    {
        private const int InitialPhaseIndex = 0;
        
        private readonly IServerStateMachineNetwork _serverStateMachine;
        private readonly GameServerPhasePayloadFactory _payloadFactory;
        private readonly IGameServerTime _serverTime;
        private readonly GamePlayersPhaseTracker _playersPhaseTracker;
        
        private readonly GamePhaseType[] _sequenceOfPhases = 
        {
            GamePhaseType.FirstMove,
            GamePhaseType.Regroup,
            GamePhaseType.Destiny,
            GamePhaseType.Launch,
            GamePhaseType.Alliance
        };

        private readonly Dictionary<GamePhaseType, Func<double?>> _transitionsByPhaseType = new();

        private (int Index, double? EndPhaseTime)? _activePhaseData;

        public GameServerPhaseTransitioner(
            IServerStateMachineNetwork serverStateMachine,
            GameServerPhasePayloadFactory payloadFactory,
            IGameServerTime serverTime,
            GamePlayersPhaseTracker playersPhaseTracker)
        {
            _serverStateMachine = serverStateMachine;
            _payloadFactory = payloadFactory;
            _serverTime = serverTime;
            _playersPhaseTracker = playersPhaseTracker;
            _serverTime.Tick += Update;
            InitTransitions();
        }

        public void Dispose()
        {
            _serverTime.Tick -= Update;
        }
        
        public void OnInitialized()
        {
            if (_activePhaseData != null)
            {
                Logger.Error($"{nameof(GameServerPhaseTransitioner)}.{nameof(OnInitialized)}: phase already started.");
                return;
            }
            
            var phaseType = _sequenceOfPhases[InitialPhaseIndex];
            var phaseTransitionAction = _transitionsByPhaseType[phaseType];
            var endPhaseTime = phaseTransitionAction.Invoke();
            _activePhaseData = (InitialPhaseIndex, endPhaseTime);
        }

        private void InitTransitions()
        {
            _transitionsByPhaseType.Clear();
            _transitionsByPhaseType[GamePhaseType.FirstMove] = GoToFirstMovePhase;
            _transitionsByPhaseType[GamePhaseType.Regroup] = GoToRegroupPhase;
            _transitionsByPhaseType[GamePhaseType.Destiny] = GoToDestinyPhase;
        }

        private void Update()
        {
            if (_activePhaseData == null)
            {
                return;
            }
            
            var (index, endPhaseTime) = _activePhaseData.Value;
            
            if (!_playersPhaseTracker.AreAllPlayersInPhase(_sequenceOfPhases[index]))
            {
                return;
            }

            if (endPhaseTime != null && _serverTime.ServerTimeInSeconds < endPhaseTime)
            {
                return;
            }

            LoadNextPhase();
        }

        private void LoadNextPhase()
        {
            if (_activePhaseData == null)
            {
                Logger.Error($"{nameof(GameServerPhaseTransitioner)}.{nameof(LoadNextPhase)}: current phase is not initialized.");
                return;
            }
            
            var activePhaseData = _activePhaseData.Value;
            var nextPhaseIndex = activePhaseData.Index + 1 < _sequenceOfPhases.Length
                ? activePhaseData.Index + 1
                : InitialPhaseIndex;
            
            var phaseType = _sequenceOfPhases[nextPhaseIndex];
            _activePhaseData = null;
            var phaseDuration = _transitionsByPhaseType[phaseType].Invoke();
            _activePhaseData = (nextPhaseIndex, phaseDuration);
        }

        private double? GoToFirstMovePhase()
        {
            _serverStateMachine.ServerTransitionTo<GameFirstMovePhase>();
            
            return null;
        }

        private double? GoToRegroupPhase()
        {
            var regroupPayload = _payloadFactory.CreateRegroupPayload();
            _serverStateMachine.ServerTransitionTo<GameRegroupPhase>(regroupPayload);
            
            return regroupPayload.EndPhaseTime;
        }

        private double? GoToDestinyPhase()
        {
            var destinyPayload = _payloadFactory.CreateDestinyPayload();
            _serverStateMachine.ServerTransitionTo<GameDestinyPhase>(destinyPayload);

            return destinyPayload.EndPhaseTime;
        }
    }
}