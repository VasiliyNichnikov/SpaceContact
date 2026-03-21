using UnityEngine;

namespace Client.UI.Dialogs.Game.Hand.ViewModels
{
    public interface IGamePlayerSpaceCardViewModel
    {
        string Title { get; }
        
        Color BackgroundColor { get; }
        
        string Value { get; }
    }
}