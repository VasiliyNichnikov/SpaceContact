using System;
using Newtonsoft.Json;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class PlayerHandSnapshot
    {
        [JsonIgnore]
        public abstract int Count { get; }
    }
}
