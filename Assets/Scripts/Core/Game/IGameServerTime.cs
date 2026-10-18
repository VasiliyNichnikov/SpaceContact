namespace Core.Game
{
    public interface IGameServerTime
    {
        delegate void Update(double deltaTimeInSeconds);

        double ServerTimeInSeconds { get; }

        void Subscribe(Update action);

        void Unsubscribe(Update action);
    }
}
