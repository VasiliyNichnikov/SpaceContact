#if UNITY_EDITOR
using System;
using Client.Configs.Menu;

namespace Editor
{
    public sealed class LobbySettingsValidation : IGameRulesValidation
    {
        public void Validate(GameRulesValidationContext context)
        {
            context.ValidateAssets<LobbySettings>(config =>
            {
                var data = config.BuildData();

                if (data.MaxNumberOfPlayers < GameRulesValidationContext.MinimumPlayerCount ||
                    data.MaxNumberOfPlayers > data.AllAvailablePlayerColors.Length)
                {
                    throw new InvalidOperationException("Lobby player limit must allow a game and a color for each player.");
                }

                context.IncludePlayerCount(data.MaxNumberOfPlayers);
            });
        }
    }
}
#endif
