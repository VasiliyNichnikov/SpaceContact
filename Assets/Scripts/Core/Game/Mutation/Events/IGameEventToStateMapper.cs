using Core.Game.Mutation.Events;

namespace Core.Game.Mutation
{
    public interface IGameEventToStateMapper<out TResult>
    {
        TResult Visit(GameServerAggressorSelectedEvent serverEvent);
        
        TResult Visit(GameServerDefenderSelectedEvent serverEvent);
    }
}