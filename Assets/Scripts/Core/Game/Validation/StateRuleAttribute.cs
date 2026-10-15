using System;

namespace Core.Game.Validation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class StateRuleAttribute : Attribute
    {
        public StateRuleAttribute(Type ruleType)
        {
            RuleType = ruleType;
        }

        public Type RuleType { get; }
    }
}
