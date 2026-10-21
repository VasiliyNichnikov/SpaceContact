using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game;
using GeneralUtils;
using Core.Game.Dto;
using Logs;
using Network.Game;
using Network.Requests;

namespace App.Game.Services
{
    public sealed class GameClientSynchronizationService : IDisposable
    {
        private readonly INetworkService _networkService;
        private readonly GameStartupNetwork _startupNetwork;
        private readonly CancellationTokenSource _disposeTokenSource = new();
        private readonly GameContext _context;
        private readonly GameRevisionAckNetwork _revisionAckNetwork;
        
        public GameClientSynchronizationService(
            GameStartupNetwork startupNetwork,
            INetworkService networkService,
            GameContext context,
            GameRevisionAckNetwork revisionAckNetwork)
        {
            _startupNetwork = startupNetwork;
            _networkService = networkService;
            _context = context;
            _revisionAckNetwork = revisionAckNetwork;
            _startupNetwork.SnapshotSyncAllowed += LoadSnapshot;
        }
        
        public void Dispose()
        {
            _startupNetwork.SnapshotSyncAllowed -= LoadSnapshot;
        }

        private void LoadSnapshot()
        {
            LoadSnapshotAsync(_disposeTokenSource.Token).FireAndForget();
        }
        
        private async Task LoadSnapshotAsync(CancellationToken ct)
        {
            if (_context.Role != GameContextRole.Client)
            {
                Logger.Error("Snapshot synchronization requires a client context.");
                return;
            }
            
            var request = new GameSnapshotRequestDto();
            var response = await _networkService.GetDataAsync<GameSnapshotRequestDto, GameSnapshotResponseDto>(
                request,
                NetworkRequestType.GetGameSnapshot,
                ct);
            
            if(ct.IsCancellationRequested)
            {
                return;
            }
            
            if (response == null)
            {
                Logger.Error($"{nameof(GameClientSynchronizationService)}.{nameof(LoadSnapshotAsync)} response returned null.");
                return;
            }
            
            _context.LoadSnapshot(response.Revision, response.Snapshot);
            _revisionAckNetwork.Send(response.Revision, _context.Phase.Revision);
        }
    }
}