using Core.Game.Cards;
using CoreConvertor;
using UnityEngine;

namespace Client.UI.HUDs.ViewModels
{
    public class GameDestinyCardViewModel : IGameDestinyCardViewModel
    {
        public GameDestinyCardViewModel(IDestinyCard card)
        {
            Description = card.Description;
            BackgroundColor = ColorConvertor.FromCoreColor(card.BackgroundColor);
        }

        public Color BackgroundColor { get; }
        
        public string Description { get; }
    }
}