using Network.Game.Mutation;

namespace Core.Game.Mutation
{
    public interface IGameEventData
    {
        void Apply(IGameEventContext context);
    }
}