using System.Collections.Generic;
using System.Linq;
using Client.Game.Field;
using Core.Game;
using Core.Game.Actions;
using Logs;
using Reactivity;
using VContainer;

namespace Client.UI.HUDs.ViewModels
{
    public sealed class GameHudViewModel : IGameHudViewModel
    {
        private readonly ReactivityProperty<IGameDestinyCardViewModel?> _destinyCardViewModel = new();
        private readonly ReactivityProperty<GamePlayerBlockViewModel> _playerBlockViewModel = new();

        [Inject] private readonly GameContext _context = null!;
        [Inject] private readonly PendingGameActions _pendingGameActions = null!;
        [Inject] private readonly GameActionClient _actionClient = null!;
        [Inject] private readonly IGameFieldViewManager _fieldViewManager = null!;
        [Inject] private readonly IGameHudTopViewModel _topViewModel = null!;
        [Inject] private readonly IGameHudBottomViewModel _bottomViewModel = null!;

        private List<GamePlayerProfileViewModel> _playerProfilesViewModels = null!;

        public IGameHudTopViewModel TopViewModel =>
            _topViewModel;

        public IGameHudBottomViewModel BottomViewModel =>
            _bottomViewModel;

        public IReactivityProperty<IGameDestinyCardViewModel?> DestinyCardViewModel =>
            _destinyCardViewModel;

        public IReactivityProperty<GamePlayerBlockViewModel> OpponentPlayerViewModel =>
            _playerBlockViewModel;

        public IReadOnlyCollection<GamePlayerProfileViewModel> PlayerProfilesViewModels =>
            _playerProfilesViewModels;

        public void Init()
        {
            _fieldViewManager.OnViewedOpponentChanged += OpponentChanged;
            _fieldViewManager.OnInitialized += OpponentChanged;
            _context.StateChanged += RefreshDestinyCard;
            _pendingGameActions.Changed += RefreshDestinyCard;
            _playerProfilesViewModels = CreatePlayerProfilesViewModels();
            _topViewModel.Init();

            OpponentChanged();
            RefreshDestinyCard();
        }

        public void Dispose()
        {
            TopViewModel.Dispose();
            _context.StateChanged -= RefreshDestinyCard;
            _pendingGameActions.Changed -= RefreshDestinyCard;
            _fieldViewManager.OnInitialized -= OpponentChanged;
            _fieldViewManager.OnViewedOpponentChanged -= OpponentChanged;

            foreach (var playerProfileViewModel in _playerProfilesViewModels)
            {
                playerProfileViewModel.Dispose();
            }

            _playerProfilesViewModels.Clear();
        }

        private void OpponentChanged()
        {
            var viewedOpponentPlayer = _fieldViewManager.ViewedOpponentPlayer;

            if (viewedOpponentPlayer == null)
            {
                Logger.Error($"{nameof(GameHudViewModel)}.{nameof(OpponentChanged)}: viewedOpponentPlayer is null.");
                return;
            }

            _playerBlockViewModel.Value = new GamePlayerBlockViewModel(viewedOpponentPlayer);
        }

        private void RefreshDestinyCard()
        {
            var localPlayer = _context.LocalPlayerState;
            if (localPlayer is null || _context.Cards.ActiveDestinyCard is null)
            {
                _destinyCardViewModel.Value = null;
                return;
            }

            _destinyCardViewModel.Value = new GameDestinyCardViewModel(
                localPlayer, _context, _pendingGameActions, SkipDestinyCard);
        }

        private List<GamePlayerProfileViewModel> CreatePlayerProfilesViewModels()
        {
            return _context
                .Players
                .All
                .Select(player => new GamePlayerProfileViewModel(player))
                .ToList();
        }

        private void SkipDestinyCard()
        {
            if (!GameDestinyCardViewModel.CanSkip(_context, _pendingGameActions))
            {
                return;
            }

            var skipDestinyCardGameAction = new SkipDestinyCardGameAction(_context.LocalPlayerId);
            _actionClient.Send(skipDestinyCardGameAction);
        }
    }
}
