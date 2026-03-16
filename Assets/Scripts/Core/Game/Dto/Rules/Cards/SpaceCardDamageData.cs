namespace Core.Game.Dto.Rules.Cards
{
    public readonly struct SpaceCardDamageData
    {
        public readonly int Count;

        public readonly int DamageCount;

        public SpaceCardDamageData(int count, int damageCount)
        {
            Count = count;
            DamageCount = damageCount;
        }
    }
}