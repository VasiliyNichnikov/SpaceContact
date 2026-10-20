namespace Core.Game.Actions
{
    public interface IActionValidator
    {
        ValidationResult Check(GameContext context, GameActor actor);
    }
}