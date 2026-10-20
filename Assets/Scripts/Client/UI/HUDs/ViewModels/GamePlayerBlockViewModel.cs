using Core.Game;
using CoreConvertor;
using Color = UnityEngine.Color;

namespace Client.UI.HUDs.ViewModels
{
    public class GamePlayerBlockViewModel
    {
        public string PlayerName { get; }
        
        public Color PlayerColor { get; }

        public GamePlayerBlockViewModel(GamePlayerData opponentPlayer)
        {
            PlayerName = $"Opponent: {opponentPlayer.Name}";
            PlayerColor = ColorConvertor.FromCoreColor(opponentPlayer.Color);
        }
    }
}