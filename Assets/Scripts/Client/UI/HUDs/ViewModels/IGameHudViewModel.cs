using Reactivity;

namespace Client.UI.HUDs.ViewModels
{
    public interface IGameHudViewModel
    {
        IGameHudTopViewModel TopViewModel { get; }
        
        IGameHudBottomViewModel BottomViewModel { get; }
        
        IReactivityProperty<IGameDestinyCardViewModel?> DestinyCardViewModel { get; }
        
        IReactivityProperty<GamePlayerBlockViewModel?> OpponentPlayerViewModel { get; }
        
        IReactivityReadOnlyCollectionProperty<GamePlayerProfileViewModel> PlayerProfilesViewModels { get; }
    }
}