using Core.Game.Encounter;
using Core.Game.Phases;
using Core.Game.Players;

namespace Core.Game.Rules
{
    public sealed class GameCanBeAggressorRule : IGameRule
    {
        private readonly IGameStateMachineReadOnly _stateMachine;
        private readonly IGameClientEncounterManager _encounterManager;
        private readonly GamePlayersRegistry _playersRegistry;
        
        public GameCanBeAggressorRule(
            IGameStateMachineReadOnly stateMachine,
            IGameClientEncounterManager encounterManager,
            GamePlayersRegistry playersRegistry)
        {
            _stateMachine = stateMachine;
            _playersRegistry = playersRegistry;
            _encounterManager = encounterManager;
        }

        public GameRuleType Type => 
            GameRuleType.CanBeAggressor;

        public bool Check(GameRuleContext context)
        {
            if (context.SelectedPlayerId == null)
            {
                return false;
            }

            if (_stateMachine.CurrentPhase is not { Type: GamePhaseType.FirstMove })
            {
                return false;
            }

            if (!_playersRegistry.ContainsPlayer(context.SelectedPlayerId.Value))
            {
                return false;
            }

            var aggressor = _encounterManager.AggressorPlayer;
            
            return aggressor == null;
        }
    }
}