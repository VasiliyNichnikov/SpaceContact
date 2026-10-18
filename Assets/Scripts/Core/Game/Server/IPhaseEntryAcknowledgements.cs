using System.Collections.Generic;

namespace Core.Game.Server
{
    /// <summary>
    /// Tracks applied state, not receipt of a message. The host acknowledges through its client context too.
    /// </summary>
    public interface IPhaseEntryAcknowledgements
    {
        /// <summary>
        /// Returns true only for a nonempty participant set when every participant has acknowledged
        /// this exact phase revision and a state revision at least as high as entryStateRevision.
        /// Missing participants and acknowledgements of previous phases cannot satisfy the barrier.
        /// </summary>
        bool AreAllPlayersAtEntry(IReadOnlyCollection<ulong> participantIds, long phaseRevision, long entryStateRevision);
    }
}
