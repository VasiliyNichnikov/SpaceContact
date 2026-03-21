namespace Core.Game.Cards
{
    public class EmptySpaceCard : ISpaceCard
    {
        private static EmptySpaceCard? _instance;
        
        private EmptySpaceCard()
        {
            // nothing
        }

        public static ISpaceCard Instance => _instance ??= new EmptySpaceCard();
    }
}