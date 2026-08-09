using Core.EngineData;
using Core.Game.Dto.Snapshots;

namespace Core.Game
{
    public sealed class GamePlayerData
    {
        public GamePlayerData(ulong id, string name, int order, Color color, bool isReady)
        {
            Id = id;
            Name = name;
            Color = color;
            Order = order;
            IsReady = isReady;
        }
        
        public ulong Id { get; }
        
        public string Name { get; }
        
        public int Order { get; }
        
        public Color Color { get; }
        
        public bool IsReady { get; private set; }

        public static GamePlayerData Convert(GamePlayerSnapshot snapshot)
        {
            return new GamePlayerData(snapshot.PlayerId, snapshot.Name, snapshot.Order, snapshot.Color, snapshot.IsReady);
        }

        public bool TryChangeIsReadyValue(bool value)
        {
            if(IsReady == value)
            {
                return false;
            }

            IsReady = value;
            return true;
        }
    }
}