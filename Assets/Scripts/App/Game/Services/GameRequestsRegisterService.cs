using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Game;
using Core.Game.Players;
using Network.Infrastructure;
using Network.Requests;

namespace App.Game.Services
{
    public class GameRequestsRegisterService : IDisposable
    {
        private readonly NetworkRequestRouter _router;
        private readonly INetworkSerializer _networkSerializer;
        
        private readonly IGalaxyManagerNetwork _galaxyManagerNetwork;
        private readonly GamePlayersRegistry _gamePlayersRegistry;
        
        private readonly ReadOnlyCollection<INetworkRequestHandler> _handlers;

        public GameRequestsRegisterService(
            NetworkRequestRouter router, 
            INetworkSerializer networkSerializer,
            
            IGalaxyManagerNetwork galaxyManagerNetwork,
            GamePlayersRegistry gamePlayersRegistry)
        {
            _router = router;
            _networkSerializer = networkSerializer;
            _galaxyManagerNetwork = galaxyManagerNetwork;
            _gamePlayersRegistry = gamePlayersRegistry;
            
            _handlers = CreateHandlers();
        }
        
        public void ConfigureRegistry() => 
            _router.Subscribe(_handlers);
        
        public void Dispose() => 
            _router.Unsubscribe(_handlers);
        
        private ReadOnlyCollection<INetworkRequestHandler> CreateHandlers()
        {
            var galaxyStateHandler = new GetGalaxyStateNetworkRequestHandler(_networkSerializer, _galaxyManagerNetwork);
            var playerHandStateHandler = new GetPlayerHandStateNetworkRequestHandler(_networkSerializer, _gamePlayersRegistry);

            var allStates = new List<INetworkRequestHandler>
            {
                galaxyStateHandler,
                playerHandStateHandler,
            };

            return allStates.AsReadOnly();
        }
    }
}