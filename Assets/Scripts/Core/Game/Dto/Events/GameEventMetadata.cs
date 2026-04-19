using System;

namespace Core.Game.Dto
{
    [Serializable]
    public struct GameEventMetadata
    {
        public int EventId;

        public double CreatedAtSeconds;
    }
}