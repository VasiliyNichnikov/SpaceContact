using System;
using Core.Game;
using Core.Game.Actions;
using CoreConvertor;
using UnityEngine;

namespace Client.UI.HUDs.ViewModels
{
    public class GameDestinyCardViewModel : IGameDestinyCardViewModel
    {
        private readonly GameContext _context;
        private readonly PendingGameActions _pendingActions;
        private readonly Action _onSkipButtonClickAction;

        public GameDestinyCardViewModel(
            GamePlayerData ownerPlayer, 
            GameContext context,
            PendingGameActions pendingActions, 
            Action onSkipButtonClickAction)
        {
            _context = context;
            _pendingActions = pendingActions;
            Description = string.Empty;
            BackgroundColor = ColorConvertor.FromCoreColor(ownerPlayer.Color);
            _onSkipButtonClickAction = onSkipButtonClickAction;
        }

        public Color BackgroundColor { get; }

        public string Description { get; }

        public bool IsSkipButtonVisible => CanSkip(_context, _pendingActions);

        public static bool CanSkip(GameContext context, PendingGameActions pendingActions)
        {
            var localPlayer = context.LocalPlayerState;
            if (localPlayer is null || pendingActions.HasAny)
            {
                return false;
            }

            var action = new SkipDestinyCardGameAction(localPlayer.Id);
            return action.GetValidator().Check(context, GameActor.Player(localPlayer.Id)).IsSuccess;
        }

        public void OnSkipButtonClickHandler()
        {
            if (IsSkipButtonVisible)
            {
                _onSkipButtonClickAction.Invoke();
            }
        }
    }
}
