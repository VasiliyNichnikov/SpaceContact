using Logs;

namespace Core.Phases
{
    public class RegroupPhase : IGamePhase
    {
        public void Enter()
        {
            Logger.Log("RegroupPhase.Enter");
        }

        public void Exit()
        {
            Logger.Log("RegroupPhase.Exit");
        }

        public void Update()
        {
            Logger.Log("RegroupPhase.Update");
        }
    }
}