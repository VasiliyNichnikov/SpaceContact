using System.Linq;
using Core.Game.Mutation;
using Core.Game.Players;
using Core.Game.Rules;
using Logs;

namespace Core.Game.Phases.Server
{
    public sealed class GameServerChangerStatePlayersReadiness
    {
        private readonly IServerEventBroadcaster _broadcaster;
        private readonly GamePlayersRegistry _playersRegistry;
        private readonly GameRulesChecker _rulesChecker;
        private readonly GameServerPhaseTransitioner _transitioner;
        private readonly GameEventsFactory _eventsFactory;

        public GameServerChangerStatePlayersReadiness(
            IServerEventBroadcaster broadcaster,
            GamePlayersRegistry playersRegistry,
            GameRulesChecker rulesChecker,
            GameServerPhaseTransitioner transitioner,
            GameEventsFactory eventsFactory)
        {
            _broadcaster = broadcaster;
            _playersRegistry = playersRegistry;
            _rulesChecker = rulesChecker;
            _transitioner = transitioner;
            _eventsFactory = eventsFactory;
        }
        
        public bool ChangePlayerReadiness(ulong playerId, bool isReady)
        {
            var context = GameRuleContext.CheckPlayer(playerId);

            switch (isReady)
            {
                case true when !_rulesChecker.Check(GameRuleType.CanPlayerChangeToReady, context):
                case false when !_rulesChecker.Check(GameRuleType.CanPlayerChangeToNotReady, context):
                    Logger.Error($"{nameof(GameServerChangerStatePlayersReadiness)}.{nameof(ChangePlayerReadiness)}: couldn't make the transition.");
                    return false;
                
                default:
                {
                    var player = _playersRegistry.GetPlayerById(playerId);
                    var eventData = _eventsFactory.CreatePlayerReadinessEvent(playerId, isReady);
                    // Последовательность важна. Сначала уведомляем, затем обновляем данные
                    _broadcaster.SendEvent(eventData, RecipientType.AllClients);
                    player.IsReadyToNextPhase = isReady;
                    CheckPlayersReadiness();
                    return true;
                }
            }
        }

        private void CheckPlayersReadiness()
        {
            var isAllPlayersReady = _playersRegistry.Players.All(player => player.IsReadyToNextPhase);

            switch (isAllPlayersReady)
            {
                case true when !_transitioner.IsReadinessTimerActive:
                    _transitioner.StartReadinessTimer();
                    break;
                case false when _transitioner.IsReadinessTimerActive:
                    _transitioner.StopReadinessTimer();
                    break;
            }
        }
    }
}