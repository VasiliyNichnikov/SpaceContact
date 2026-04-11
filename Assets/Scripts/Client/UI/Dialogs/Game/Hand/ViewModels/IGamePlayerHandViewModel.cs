using Reactivity;

namespace Client.UI.Dialogs.Game.Hand.ViewModels
{
    public interface IGamePlayerHandViewModel
    {
        IReactivityProperty<bool> IsVisible { get; }
        
        IReactivityReadOnlyCollectionProperty<IGamePlayerSpaceCardViewModel> CardsViewModels { get; }
    }
}