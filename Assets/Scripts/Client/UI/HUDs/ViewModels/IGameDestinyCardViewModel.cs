using UnityEngine;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameDestinyCardViewModel
    {
        Color BackgroundColor { get; }
        
        string Description { get; }
    }
}