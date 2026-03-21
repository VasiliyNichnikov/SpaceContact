using Core.Game.Cards;
using UnityEngine;
using Logger = Logs.Logger;

namespace Client.UI.Dialogs.Game.Hand.ViewModels
{
    public class GamePlayerSpaceCardViewModel : IGamePlayerSpaceCardViewModel
    {
        public GamePlayerSpaceCardViewModel(ISpaceCard spaceCard)
        {
            var data = InitTestValue(spaceCard);
            Title = data.Title;
            Value = data.Value;
            BackgroundColor = data.BackgroundColor;
        }
        
        public string Title { get; }
        
        public Color BackgroundColor { get; }
        
        public string Value { get; }

        private (string Title, Color BackgroundColor, string Value) InitTestValue(ISpaceCard spaceCard)
        {
            switch (spaceCard)
            {
                case DamageSpaceCard damage:
                    return ("Damage", Color.orange, damage.DamageCount.ToString());
                
                case ArtifactSpaceCard:
                    return ("Artifact", Color.blue, "Artifact");
                
                case ConversationSpaceCard:
                    return ("Conversation",  Color.green, "Conversation");
            }
            
            Logger.Error("GamePlayerSpaceCardViewModel.InitTestValue: spaceCard is invalid.");
            return ("ERROR", Color.red, "ERROR");
        }
    }
}