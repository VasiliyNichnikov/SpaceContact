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
        private readonly ClientGameContext _context;
        private readonly GameActionClient _actionClient;

        public GameDestinyCardViewModel(
            DestinyCardData card, 
            ClientGameContext context,
            GameActionClient actionClient)
        {
            _card = card;
            _context = context;
            _actionClient = actionClient;
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

        public bool IsSkipButtonVisible => _actionClient.ValidateAction(CreateSkipAction());

        public void OnSkipButtonClickHandler()
        {
            var action = CreateSkipAction();
            
            if (_actionClient.ValidateAction(action))
            {
                _actionClient.Send(action);
            }
        }

        private SkipDestinyCardGameAction CreateSkipAction()
        {
            return new SkipDestinyCardGameAction(
                _context.LocalPlayerId, 
                _context.Phase.Revision,
                _context.Cards.DestinyDeckCount);
        }

        private GamePlayerData? GetSelectedPlayer()
        {
            return _card.SelectedPlayerId != null && _context.Players.IsPlayerExist(_card.SelectedPlayerId.Value)
                ? _context.Players.Get(_card.SelectedPlayerId.Value)
                : null;
        }
    }
}
