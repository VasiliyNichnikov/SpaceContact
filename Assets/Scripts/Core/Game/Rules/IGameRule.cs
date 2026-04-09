namespace Core.Game.Rules
{
    public interface IGameRule
    {
        GameRuleType Type { get; }
        
        bool Check(GameRuleContext context);
    }
}