using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Game.Encounter;
using Core.Game.Galaxy.Server;
using Core.Game.Phases.Server;
using Core.Game.Players;
using Network.Infrastructure;
using Network.Requests;

namespace App.Game.Services
{
    public sealed class GameServerRequestsRegisterService : IDisposable
    {
        private readonly NetworkRequestRouter _router;
        private readonly INetworkSerializer _networkSerializer;
        
        private readonly IGameServerGalaxyManager _serverGalaxyManager;
        private readonly IGameServerDestinyPhaseResolver _serverDestinyPhaseResolver;
        private readonly IGameServerEncounterManager _serverEncounterManager;
        private readonly GameServerChangerStatePlayersReadiness _changerStatePlayersReadiness;
        
        private readonly GamePlayersRegistry _gamePlayersRegistry;
        
        private readonly ReadOnlyCollection<INetworkRequestHandler> _handlers;

        public GameServerRequestsRegisterService(
            NetworkRequestRouter router, 
            INetworkSerializer networkSerializer,
            
            IGameServerGalaxyManager serverGalaxyManager,
            IGameServerDestinyPhaseResolver serverDestinyPhaseResolver,
            IGameServerEncounterManager serverEncounterManager,
            GamePlayersRegistry gamePlayersRegistry,
            GameServerChangerStatePlayersReadiness changerStatePlayersReadiness)
        {
            _router = router;
            _networkSerializer = networkSerializer;
            _gamePlayersRegistry = gamePlayersRegistry;
            _serverGalaxyManager = serverGalaxyManager;
            _serverDestinyPhaseResolver = serverDestinyPhaseResolver;
            _serverEncounterManager = serverEncounterManager;
            _changerStatePlayersReadiness = changerStatePlayersReadiness;
            
            _handlers = CreateHandlers();
        }
        
        public void ConfigureRegistry() => 
            _router.Subscribe(_handlers);
        
        public void Dispose() => 
            _router.Unsubscribe(_handlers);
        
        private ReadOnlyCollection<INetworkRequestHandler> CreateHandlers()
        {
            var galaxyStateHandler = new GetGalaxyStateNetworkRequestHandler(_networkSerializer, _serverGalaxyManager);
            var playerHandStateHandler = new GetPlayerHandStateNetworkRequestHandler(
                _networkSerializer,
                _gamePlayersRegistry);
            var skipDestinyCardHandler = new SkipDestinyCardNetworkRequestHandler(
                _networkSerializer,
                _serverDestinyPhaseResolver);
            var choosePlanetToAttackHandler = new ChoosePlanetToAttackNetworkRequestHandler(
                _networkSerializer,
                _serverEncounterManager);
            var changeReadinessHandler = new ChangePlayerReadinessRequestHandler(
                _changerStatePlayersReadiness,
                _networkSerializer);
            
            var allStates = new List<INetworkRequestHandler>
            {
                galaxyStateHandler,
                playerHandStateHandler,
                skipDestinyCardHandler,
                choosePlanetToAttackHandler,
                changeReadinessHandler,
            };

            return allStates.AsReadOnly();
        }
    }
}