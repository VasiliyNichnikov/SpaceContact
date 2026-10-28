using System;
using System.Linq;
using Client.Game.Field;
using Core.Game;
using Core.Game.Actions;
using Reactivity;
using VContainer;
using VContainer.Unity;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudViewModel : IGameHudViewModel, IInitializable, IDisposable
    {
        private readonly ReactivityProperty<IGameDestinyCardViewModel?> _destinyCardViewModel = new();
        private readonly ReactivityProperty<GamePlayerBlockViewModel?> _playerBlockViewModel = new();

        [Inject] private readonly ClientGameContext _context = null!;
        [Inject] private readonly PendingGameActions _pendingGameActions = null!;
        [Inject] private readonly GameActionClient _actionClient = null!;
        [Inject] private readonly IGameFieldViewManager _fieldViewManager = null!;
        [Inject] private readonly IGameHudTopViewModel _topViewModel = null!;
        [Inject] private readonly IGameHudBottomViewModel _bottomViewModel = null!;

        private readonly ReactivityListProperty<GamePlayerProfileViewModel> _playerProfilesViewModels = new(new());
        private GamePlayerData[] _profilePlayers = Array.Empty<GamePlayerData>();
        private GamePlayerProfileViewModel[] _profiles = Array.Empty<GamePlayerProfileViewModel>();

        public IGameHudTopViewModel TopViewModel =>
            _topViewModel;

        public IGameHudBottomViewModel BottomViewModel =>
            _bottomViewModel;

        public IReactivityProperty<IGameDestinyCardViewModel?> DestinyCardViewModel =>
            _destinyCardViewModel;

        public IReactivityProperty<GamePlayerBlockViewModel?> OpponentPlayerViewModel =>
            _playerBlockViewModel;

        public IReactivityReadOnlyCollectionProperty<GamePlayerProfileViewModel> PlayerProfilesViewModels =>
            _playerProfilesViewModels;

        public void Dispose()
        {
            _context.StateChanged -= Refresh;
            _pendingGameActions.Changed -= RefreshDestinyCard;
            _fieldViewManager.OnInitialized -= OpponentChanged;
            _fieldViewManager.OnViewedOpponentChanged -= OpponentChanged;
        }

        void IInitializable.Initialize()
        {
            _fieldViewManager.OnViewedOpponentChanged += OpponentChanged;
            _fieldViewManager.OnInitialized += OpponentChanged;
            _context.StateChanged += Refresh;
            _pendingGameActions.Changed += RefreshDestinyCard;

            Refresh();
        }

        private void Refresh()
        {
            RefreshProfiles();
            OpponentChanged();
            RefreshDestinyCard();
        }

        private void OpponentChanged()
        {
            var viewedPlayer = _fieldViewManager.ViewedOpponentPlayer;
            if (_context.LocalPlayerState is null || viewedPlayer is null ||
                !_context.Players.IsPlayerExist(viewedPlayer.Id))
            {
                _playerBlockViewModel.Value = null;
                return;
            }

            _playerBlockViewModel.Value = new GamePlayerBlockViewModel(_context.Players.Get(viewedPlayer.Id));
        }

        private void RefreshDestinyCard()
        {
            var card = _context.Cards.ActiveDestinyCard;
            if (_context.LocalPlayerState is null || card is null)
            {
                _destinyCardViewModel.Value = null;
                return;
            }

            _destinyCardViewModel.Value = new GameDestinyCardViewModel(card, _context, _actionClient);
        }

        private void RefreshProfiles()
        {
            var players = _context.LocalPlayerState is null
                ? Array.Empty<GamePlayerData>()
                : _context.Players.All.OrderBy(player => player.Order).ToArray();
            var playersChanged = !_profilePlayers.SequenceEqual(players);
            if (playersChanged)
            {
                _profilePlayers = players;
                _profiles = players.Select(player => new GamePlayerProfileViewModel(player)).ToArray();
            }

            foreach (var profile in _profiles)
            {
                profile.Refresh(_context.Encounter);
            }

            if (playersChanged)
            {
                _playerProfilesViewModels.Value = _profiles;
            }
        }
    }
}
