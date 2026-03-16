using System;

namespace Core.Game.Dto.States.Cards
{
    [Serializable]
    public struct DestinyCardStateData
    {
        public bool IsJoker;

        public ulong? SelectedPlayerId;
    }
}