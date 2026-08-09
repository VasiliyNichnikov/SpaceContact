using System;
using System.Collections.Generic;

namespace Core.Game.Dto.Snapshots
{
    [Serializable]
    public sealed class GamePlayersSnapshot
    {
        public List<GamePlayerSnapshot> Players = new();
    }
}