using System;
using Core.Game.Players;
using CoreConvertor;
using Reactivity;
using UnityEngine;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GamePlayerProfileViewModel : IDisposable
    {
        private readonly ReactivityProperty<bool> _isDefaultFrameVisible = new(true);
        private readonly ReactivityProperty<bool> _isAttackerFrameVisible = new();
        private readonly ReactivityProperty<bool> _isDefenderFrameVisible = new();
        
        public GamePlayerProfileViewModel(IGamePlayer player)
        {
            PlayerColor = ColorConvertor.FromCoreColor(player.Color);
        }
        
        public Color PlayerColor { get; }

        public IReactivityProperty<bool> IsDefaultFrameVisible => 
            _isDefaultFrameVisible;

        public IReactivityProperty<bool> IsAttackerFrameVisible => 
            _isAttackerFrameVisible;

        public IReactivityProperty<bool> IsDefenderFrameVisible => 
            _isDefenderFrameVisible;

        public void Dispose()
        {
            // nothing
        }
    }
}