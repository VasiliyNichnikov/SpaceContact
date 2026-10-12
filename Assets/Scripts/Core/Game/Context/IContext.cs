namespace Core.Game
{
    /// <summary>
    /// Converts one section without publishing a completed game state.
    /// External snapshots must be loaded through GameContext to install all sections together.
    /// ToSnapshot returns collections owned by the caller.
    /// </summary>
    public interface IContext<TSnapshot>
    {
        void LoadSnapshot(TSnapshot snapshot);
        
        TSnapshot ToSnapshot();
    }
}
