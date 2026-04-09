using Core.Game.Encounter;
using Core.Game.Phases;
using Core.Game.Players;

namespace Core.Game.Rules
{
    public sealed class GameCanBeDefenderRule : IGameRule
    {
        private readonly IGameStateMachineReadOnly _stateMachine;
        private readonly IGameClientEncounterManager _encounterManager;
        private readonly GamePlayersRegistry _playersRegistry;

        public GameCanBeDefenderRule(
            IGameStateMachineReadOnly stateMachine,
            GamePlayersRegistry playersRegistry,
            IGameClientEncounterManager encounterManager)
        {
            _stateMachine = stateMachine;
            _playersRegistry = playersRegistry;
            _encounterManager = encounterManager;
        }
        
        public GameRuleType Type => 
            GameRuleType.CanBeDefender;

        public bool Check(GameRuleContext context)
        {
            if (context.SelectedPlayerId == null)
            {
                return false;
            }

            if (_stateMachine.CurrentPhase is not { Type: GamePhaseType.Destiny })
            {
                return false;
            }
            
            if (!_playersRegistry.ContainsPlayer(context.SelectedPlayerId.Value))
            {
                return false;
            }

            var aggressor = _encounterManager.AggressorPlayer;
            
            if (aggressor == null || aggressor.PlayerId == context.SelectedPlayerId)
            {
                return false;
            }

            return _encounterManager.DefenderPlayer == null;
        }
    }
}