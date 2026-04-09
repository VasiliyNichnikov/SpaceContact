using System;

namespace Network.Dto
{
    [Serializable]
    public struct GameEventMetadata
    {
        public int EventId;

        public int CreatedAtSeconds;
    }
}