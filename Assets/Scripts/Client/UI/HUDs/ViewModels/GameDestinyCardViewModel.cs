using System;
using Core.Game;
using Core.Game.Actions;
using CoreConvertor;
using UnityEngine;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameDestinyCardViewModel : IGameDestinyCardViewModel
    {
        private readonly DestinyCardData _card;
        private readonly GameContext _context;
        private readonly PendingGameActions _pendingActions;
        private readonly Action _onSkipButtonClickAction;

        public GameDestinyCardViewModel(
            DestinyCardData card, 
            GameContext context, 
            PendingGameActions pendingActions, 
            Action onSkipButtonClickAction)
        {
            _card = card;
            _context = context;
            _pendingActions = pendingActions;
            _onSkipButtonClickAction = onSkipButtonClickAction;
        }

        public Color BackgroundColor
        {
            get
            {
                var player = GetSelectedPlayer();
                return player is null ? Color.white : ColorConvertor.FromCoreColor(player.Color);
            }
        }

        public string Description
        {
            get
            {
                if (_card.IsJoker)
                {
                    return "Joker: choose a planet in another player's system.";
                }

                var player = GetSelectedPlayer();
                if (player is not null)
                {
                    return player.Id == _context.Encounter.AggressorPlayerId
                        ? $"{player.Name}: choose an enemy on a planet in {player.Name}'s system."
                        : $"Choose a planet in {player.Name}'s system.";
                }

                return "Unsupported Destiny card.";
            }
        }

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

        private GamePlayerData? GetSelectedPlayer()
        {
            return _card.SelectedPlayerId != null && _context.Players.IsPlayerExist(_card.SelectedPlayerId.Value)
                ? _context.Players.Get(_card.SelectedPlayerId.Value)
                : null;
        }
    }
}
