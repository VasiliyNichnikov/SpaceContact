namespace Core.Game.Actions
{
    public sealed class GameActionExecutor
    {
        private readonly GameContext _context;
        
        public GameActionExecutor(
            GameContext context)
        {
            _context = context;
        }
        
        public GameActionResult Execute(IGameAction action, GameActor actor)
        {
            var result = action.Execute(_context, actor);

            if (!result.IsSuccess)
            {
                return result;
            }

            foreach (var evt in result.Events)
            {
                evt.Apply(_context);
            }
            
            return result;
        }
        
    }
}