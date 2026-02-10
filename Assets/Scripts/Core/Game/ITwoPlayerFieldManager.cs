namespace Core.Game
{
    public interface ITwoPlayerFieldManager
    {
        /// <summary>
        /// Информация о нашем игроке за которого играем
        /// </summary>
        public GamePlayer CurrentPlayer { get; }
        
        /// <summary>
        /// Информация об игроке, который на против нас
        /// </summary>
        public GamePlayer OpponentPlayer { get; }

        void Init();
    }
}