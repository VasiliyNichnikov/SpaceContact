using Core.Game.Players;
using CoreConvertor;
using Color = UnityEngine.Color;

namespace Client.UI.HUDs.ViewModels
{
    public class GamePlayerBlockViewModel
    {
        public string PlayerName { get; }
        
        public Color PlayerColor { get; }

        public GamePlayerBlockViewModel(IGamePlayer opponentPlayer)
        {
            PlayerName = $"Opponent: {opponentPlayer.PlayerName}";
            PlayerColor = ColorConvertor.FromCoreColor(opponentPlayer.Color);
        }
    }
}