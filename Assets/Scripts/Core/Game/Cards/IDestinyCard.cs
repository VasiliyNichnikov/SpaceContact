using Core.EngineData;

namespace Core.Game.Cards
{
    /// <summary>
    /// Карта судьбы
    /// </summary>
    public interface IDestinyCard
    {
        string Description { get; }
        
        Color BackgroundColor { get; }
    }
}