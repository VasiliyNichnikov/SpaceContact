using System.Threading;
using System.Threading.Tasks;
using Core.Game.Players;
using Core.Game.Players.Visitors;
using Logs;

namespace Core.Game.Cards
{
    public sealed class GameClientPlayerCardsDeckService : IGameClientPlayerCardsDeckService
    {
        private readonly GamePlayersRegistry _playersRegistry;
        private readonly IGamePlayerHandServerInteraction _serverInteraction;
        
        public GameClientPlayerCardsDeckService(
            GamePlayersRegistry playersRegistry,
            IGamePlayerHandServerInteraction serverInteraction)
        {
            _playersRegistry = playersRegistry;
            _serverInteraction = serverInteraction;
        }
        
        public async Task InitPlayersHands(CancellationToken ct)
        {
            foreach (var player in _playersRegistry.Players)
            {
                var handState = await _serverInteraction.CreatePlayerHandAsync(player.PlayerId, ct);

                if (handState == null)
                {
                    Logger.Error($"{nameof(GameClientPlayerCardsDeckService)}.{nameof(InitPlayersHands)}: handState is null.");
                    continue;
                }

                if (ct.IsCancellationRequested)
                {
                    return;
                }
                
                var handDistributionVisitor = new GamePlayerHandDistributionVisitor(handState);
                player.Apply(handDistributionVisitor);
            }
        }
    }
}