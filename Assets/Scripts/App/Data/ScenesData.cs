namespace App.Data
{
    public record ScenesData
    {
        public readonly string MenuSceneName;
        
        public readonly string GameSceneName;

        public ScenesData(string menuSceneName, string gameSceneName)
        {
            MenuSceneName = menuSceneName;
            GameSceneName = gameSceneName;
        }
    }
}