namespace Core.Game.Phases.Server
{
    public interface IGameServerDestinyPhaseResolver
    {
        void ChooseDestiny();
        
        bool SkipDestiny(ulong senderId);
    }
}