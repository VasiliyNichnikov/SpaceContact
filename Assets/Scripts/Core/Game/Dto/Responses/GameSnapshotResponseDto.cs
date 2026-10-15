using System;
using Core.Game.Dto.Snapshots;
using Core.Game.Validation;

namespace Core.Game.Dto
{
    [Serializable]
    public class GameSnapshotResponseDto
    {
        [StateRequired]
        public GameSnapshot Snapshot = null!;

        [StateRequired]
        public long Revision;
    }
}
