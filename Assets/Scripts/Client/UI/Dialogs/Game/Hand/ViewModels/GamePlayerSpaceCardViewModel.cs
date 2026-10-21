using Core.Game;
using UnityEngine;

namespace Client.UI.Dialogs.Game.Hand.ViewModels
{
    public sealed class GamePlayerSpaceCardViewModel : IGamePlayerSpaceCardViewModel
    {
        private readonly SpaceCardData _spaceCard;

        public GamePlayerSpaceCardViewModel(SpaceCardData spaceCard)
        {
            _spaceCard = spaceCard;
        }

        public string Title => _spaceCard switch
        {
            { DamageCount: not null } => "Damage",
            { ArtifactId: not null } => "Artifact",
            { IsConversation: true } => "Conversation",
            _ => "ERROR"
        };

        public Color BackgroundColor => _spaceCard switch
        {
            { DamageCount: not null } => Color.orange,
            { ArtifactId: not null } => Color.blue,
            { IsConversation: true } => Color.green,
            _ => Color.red
        };

        public string Value => _spaceCard.DamageCount is int damageCount ? damageCount.ToString() : Title;
    }
}
