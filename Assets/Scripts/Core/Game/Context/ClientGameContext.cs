using Core.Game.Dto.Phases;

namespace Core.Game
{
    public sealed class ClientGameContext : GameContext
    {
        public ClientGameContext(IGameServerTime time, IGameDataProvider dataProvider, GamePhaseDurationData phaseDurations)
            : base(time, dataProvider, GameContextRole.Client, phaseDurations)
        {
        }
    }
}
