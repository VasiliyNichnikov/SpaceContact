using Core.EngineData;

namespace Core.Game.Cards
{
    public class ErrorDestinyCard : IDestinyCard
    {
        private static ErrorDestinyCard? _instance;
        
        private ErrorDestinyCard()
        {
            // nothing
        }

        public static IDestinyCard Instance => _instance ??= new ErrorDestinyCard();
        
        public string Description => 
            "Card is not supported.";

        public Color BackgroundColor => Color.FromHex("#FF0000");
    }
}