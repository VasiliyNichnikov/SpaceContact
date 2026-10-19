using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using Core.Game.Server;

namespace Network.Game
{
    public sealed class ServerClientRevisionTracker : IPhaseEntryAcknowledgements
    {
        private const long UnacknowledgedRevision = -1;

        private readonly Dictionary<ulong, long> _revisionByClientId = new();
        private readonly Dictionary<ulong, long> _phaseByClientId = new();
        private GameContext? _context;

        public event Action<ulong, long>? ClientRevisionChanged;

        public void Initialize(GameContext context)
        {
            if (context.Role != GameContextRole.Server || !context.IsInitialized)
            {
                throw new InvalidOperationException("Revision tracking requires an initialized server context.");
            }

            _context = context;
            _revisionByClientId.Clear();
            _phaseByClientId.Clear();
            foreach (var player in context.Players.All)
            {
                _revisionByClientId.Add(player.Id, UnacknowledgedRevision);
                _phaseByClientId.Add(player.Id, UnacknowledgedRevision);
            }
        }

        public void Acknowledge(ulong clientId, long revision, long phaseRevision)
        {
            var context = _context;
            
            if (context is null || !_revisionByClientId.TryGetValue(clientId, out var currentRevision) ||
                revision < 0 || revision > context.Revision || revision < currentRevision ||
                phaseRevision < 0 || phaseRevision > context.Phase.Revision || phaseRevision > revision ||
                phaseRevision < _phaseByClientId[clientId])
            {
                return;
            }

            if (revision == currentRevision && phaseRevision == _phaseByClientId[clientId])
            {
                return;
            }

            _revisionByClientId[clientId] = revision;
            _phaseByClientId[clientId] = phaseRevision;
            ClientRevisionChanged?.Invoke(clientId, revision);
        }

        public bool AreAllClientsAtRevision(long requiredRevision) =>
            AreAllClientsAtRevision(_revisionByClientId.Keys, requiredRevision);

        public bool AreAllClientsAtRevision(IReadOnlyCollection<ulong> requiredClientIds, long requiredRevision) =>
            requiredClientIds.Count > 0 && requiredRevision >= 0 && requiredClientIds.All(clientId =>
                _revisionByClientId.TryGetValue(clientId, out var revision) && revision >= requiredRevision);

        public bool AreAllPlayersAtEntry(IReadOnlyCollection<ulong> participantIds, long phaseRevision, long entryStateRevision) =>
            phaseRevision >= 0 && AreAllClientsAtRevision(participantIds, entryStateRevision) &&
            participantIds.All(clientId => _phaseByClientId[clientId] == phaseRevision);
    }
}
