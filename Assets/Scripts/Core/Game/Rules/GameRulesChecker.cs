using System.Collections.Generic;

namespace Core.Game.Rules
{
    public sealed class GameRulesChecker
    {
        private readonly Dictionary<GameRuleType, IGameRule> _rules = new();
        
        public GameRulesChecker(IEnumerable<IGameRule> rules)
        {
            foreach (var rule in rules)
            {
                _rules.Add(rule.Type, rule);
            }
        }

        public bool Check(GameRuleType ruleType, GameRuleContext context)
        {
            return !_rules.TryGetValue(ruleType, out var rule) || rule.Check(context);
        }

        public bool Check(IReadOnlyCollection<GameRuleType> ruleTypes, GameRuleContext context)
        {
            var isVerified = false;

            foreach (var ruleType in ruleTypes)
            {
                isVerified &= Check(ruleType, context);

                if (!isVerified)
                {
                    return false;
                }
            }

            return isVerified;
        }
    }
}