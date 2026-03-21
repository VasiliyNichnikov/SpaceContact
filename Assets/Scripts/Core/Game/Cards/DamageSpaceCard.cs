namespace Core.Game.Cards
{
    public class DamageSpaceCard : ISpaceCard
    {
        public DamageSpaceCard(int damageCount)
        {
            DamageCount = damageCount;
        }
        
        public int DamageCount { get; }
    }
}