using Core.Game;
using CoreConvertor;
using Color = UnityEngine.Color;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GamePlayerBlockViewModel
    {
        private readonly GamePlayerData _player;

        public GamePlayerBlockViewModel(GamePlayerData player)
        {
            _player = player;
        }

        public string PlayerName => $"Opponent: {_player.Name}";

        public Color PlayerColor => ColorConvertor.FromCoreColor(_player.Color);
    }
}
