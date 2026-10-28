using Core.Game.Dto.Phases;

namespace Core.Game
{
    public sealed class ServerGameContext : GameContext
    {
        public ServerGameContext(
            IGameServerTime time,
            IGameDataProvider dataProvider,
            GamePhaseDurationData phaseDurations)
            : base(time, dataProvider, GameContextRole.Server, phaseDurations)
        {
        }
    }
}
