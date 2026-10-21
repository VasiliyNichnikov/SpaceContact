using Core.Game;
using CoreConvertor;
using Reactivity;
using UnityEngine;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GamePlayerProfileViewModel
    {
        private readonly GamePlayerData _player;
        private readonly ReactivityProperty<bool> _isDefaultFrameVisible = new(true);
        private readonly ReactivityProperty<bool> _isAttackerFrameVisible = new();
        private readonly ReactivityProperty<bool> _isDefenderFrameVisible = new();

        public GamePlayerProfileViewModel(GamePlayerData player)
        {
            _player = player;
        }

        public Color PlayerColor => ColorConvertor.FromCoreColor(_player.Color);

        public IReactivityProperty<bool> IsDefaultFrameVisible => _isDefaultFrameVisible;

        public IReactivityProperty<bool> IsAttackerFrameVisible => _isAttackerFrameVisible;

        public IReactivityProperty<bool> IsDefenderFrameVisible => _isDefenderFrameVisible;

        public void Refresh(EncounterContext encounter)
        {
            var isAttacker = encounter.AggressorPlayerId == _player.Id;
            var isDefender = encounter.DefenderPlayerId == _player.Id;
            _isAttackerFrameVisible.Value = isAttacker;
            _isDefenderFrameVisible.Value = isDefender;
            _isDefaultFrameVisible.Value = !isAttacker && !isDefender;
        }
    }
}
