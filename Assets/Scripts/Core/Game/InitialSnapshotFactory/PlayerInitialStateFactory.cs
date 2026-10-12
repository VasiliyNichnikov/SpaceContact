using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Snapshots;
using Core.Game.InitialSnapshotFactory;

namespace Core.Game.Players
{
    public sealed class PlayerInitialStateFactory
    {
        private const int MinimumPlayerCount = 2;

        public GamePlayersSnapshot Create(IReadOnlyCollection<GameParticipantData> participants)
        {
            if (participants.Count < MinimumPlayerCount)
            {
                throw new ArgumentException($"At least {MinimumPlayerCount} participants are required.", nameof(participants));
            }

            var playerIds = new HashSet<ulong>();
            var seatNumbers = new HashSet<int>();

            foreach (var participant in participants)
            {
                if (!playerIds.Add(participant.PlayerId))
                {
                    throw new ArgumentException($"Duplicate participant ID {participant.PlayerId}.", nameof(participants));
                }

                if (participant.SeatNumber <= 0 || !seatNumbers.Add(participant.SeatNumber))
                {
                    throw new ArgumentException("Participant seats must be positive and unique.", nameof(participants));
                }
            }

            return new GamePlayersSnapshot
            {
                Players = participants.OrderBy(participant => participant.SeatNumber)
                    .Select((participant, order) => new GamePlayerSnapshot
                    {
                        PlayerId = participant.PlayerId,
                        Name = participant.Name,
                        Order = order,
                        Color = participant.Color,
                        IsReady = false
                    }).ToList()
            };
        }
    }
}
