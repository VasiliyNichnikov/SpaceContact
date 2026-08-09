using System;
using Core.EngineData;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    public struct GamePlayerSnapshot
    {
        public ulong PlayerId;

        public string Name;

        public int Order;

        public Color Color;

        public bool IsReady;
    }
}