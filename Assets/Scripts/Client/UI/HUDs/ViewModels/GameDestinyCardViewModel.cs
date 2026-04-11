using System;
using Core.Game.Cards;
using Core.Game.Players;
using Core.Game.Rules;
using CoreConvertor;
using UnityEngine;

namespace Client.UI.HUDs.ViewModels
{
    public class GameDestinyCardViewModel : IGameDestinyCardViewModel
    {
        private readonly IGamePlayer _ownerPlayer;
        private readonly IDestinyCard _card;
        private readonly GameRulesChecker _rulesChecker;
        private readonly Action _onSkipButtonClickAction;
        
        public GameDestinyCardViewModel(
            IGamePlayer ownerPlayer,
            IDestinyCard card, 
            GameRulesChecker rulesChecker,
            Action onSkipButtonClickAction)
        {
            _card = card;
            _ownerPlayer = ownerPlayer;
            _rulesChecker = rulesChecker;
            Description = card.Description;
            BackgroundColor = ColorConvertor.FromCoreColor(card.BackgroundColor);
            _onSkipButtonClickAction = onSkipButtonClickAction;
        }

        public Color BackgroundColor { get; }
        
        public string Description { get; }

        public bool IsSkipButtonVisible
        {
            get
            {
                if (_card.TargetPlayerId == null)
                {
                    return false;
                }

                var context = GameRuleContext.CheckPlayer(_ownerPlayer.PlayerId);
                
                return _rulesChecker.Check(GameRuleType.CanSkipDestinyCard, context);
            }
        }
        
        public void OnSkipButtonClickHandler() => 
            _onSkipButtonClickAction.Invoke();
    }
}