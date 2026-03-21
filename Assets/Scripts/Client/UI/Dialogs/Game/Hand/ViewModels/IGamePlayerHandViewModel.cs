using Reactivity;

namespace Client.UI.Dialogs.Game.Hand.ViewModels
{
    public interface IGamePlayerHandViewModel
    {
        IReactivityReadOnlyCollectionProperty<IGamePlayerSpaceCardViewModel> CardsViewModels { get; }
    }
}