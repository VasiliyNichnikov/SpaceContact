using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.UI.Dialogs.Lobby.ViewModels;
using Core.User;
using GeneralUtils;
using Logs;
using Reactivity;

namespace Client.UI.Dialogs.Lobby
{
    public sealed class LobbyViewModel : IDisposable
    {
        private readonly ClientUsersRepository _usersRepository;
        private readonly Dictionary<ulong, LobbyPlayerViewModel> _lobbyViewModelByUser;
        private readonly EventProvider _refreshPlayersEvent = new();
        private readonly IGameLevelControl _levelControl;
        private readonly ILobbyController _lobbyController;
        private readonly IUsersColorProvider _usersColorProvider;
        private readonly IUsersSeatsProvider _usersSeatsProvider;
        private readonly IUserServerInteraction _userServerInteraction;
        
        private bool _isWaitingResponse;
        
        public LobbyViewModel(
            ClientUsersRepository usersRepository, 
            IGameLevelControl levelControl, 
            ILobbyController lobbyController,
            IUsersColorProvider usersColorProvider,
            IUsersSeatsProvider usersSeatsProvider,
            IUserServerInteraction userServerInteraction)
        {
            _usersRepository = usersRepository;
            _levelControl = levelControl;
            _lobbyController = lobbyController;
            _usersColorProvider = usersColorProvider;
            _usersSeatsProvider = usersSeatsProvider;
            _userServerInteraction = userServerInteraction;
            _usersRepository.OnUserJoined += UserJoined;
            _usersRepository.OnUserLeft += UserLeft;
            _usersSeatsProvider.SlotsChanged += _refreshPlayersEvent.Call;
            _lobbyViewModelByUser = CreatePlayers();
        }

        public bool IsOwnerLobby => 
            _lobbyController.IsOwnerLobby;
        
        public IReadOnlyCollection<ILobbySlotViewModel> Slots => 
            CreateSlots();

        public IEventProvider RefreshPlayersEvent => 
            _refreshPlayersEvent;

        public void RunBattleButtonClickHandler() => 
            _levelControl.StartGame();
        
        public void Dispose()
        {
            _usersRepository.OnUserJoined -= UserJoined;
            _usersRepository.OnUserLeft -= UserLeft;
            _usersSeatsProvider.SlotsChanged -= _refreshPlayersEvent.Call;

            foreach (var kvp in _lobbyViewModelByUser)
            {
                kvp.Value.Dispose();
            }
            
            _lobbyViewModelByUser.Clear();
        }

        private void UserJoined(IUser user)
        {
            _lobbyViewModelByUser[user.ClientId] = CreateLobbyPlayer(user);
            _refreshPlayersEvent.Call();
        }

        private void UserLeft(IUser user)
        {
            _lobbyViewModelByUser[user.ClientId].Dispose();
            _lobbyViewModelByUser.Remove(user.ClientId);
            _refreshPlayersEvent.Call();
        }

        private Dictionary<ulong, LobbyPlayerViewModel> CreatePlayers()
        {
            var result = new Dictionary<ulong, LobbyPlayerViewModel>();
            
            foreach (var user in _usersRepository.Users)
            {
                result[user.ClientId] = CreateLobbyPlayer(user);
            }
            
            return result;
        }

        private LobbyPlayerViewModel CreateLobbyPlayer(IUser user)
        {
            return new LobbyPlayerViewModel(user, _usersColorProvider, _userServerInteraction);
        }
        
        private IReadOnlyCollection<ILobbySlotViewModel> CreateSlots()
        {
            var slotViewModels = new List<ILobbySlotViewModel>();
            
            for (var index = 0; index < _usersSeatsProvider.Seats.Length; index++)
            {
                var userId = _usersSeatsProvider.Seats[index];
                var seatNumber = index + 1;
                ILobbySlotViewModel slotViewModel;
                
                if (userId == null)
                {
                    slotViewModel = new LobbyEmptySlotViewModel(seatNumber, SelectAnotherSeatNumber);
                }
                else
                {
                    slotViewModel = _lobbyViewModelByUser[userId.Value];
                }
                
                slotViewModels.Add(slotViewModel);
            }

            return slotViewModels;
        }

        private void SelectAnotherSeatNumber(int seatNumber)
        {
            if (_isWaitingResponse)
            {
                return;
            }
            
            SelectAnotherSeatNumberAsync(seatNumber).FireAndForget();
        }
        
        private async Task SelectAnotherSeatNumberAsync(int seatNumber)
        {
            if (_isWaitingResponse)
            {
                Logger.Error("LobbyViewModel.SelectAnotherSeatNumberAsync: wait for a response from the server.");
                return;
            }
            
            _isWaitingResponse = true;
            await _userServerInteraction.ChangeMySeatNumberAsync(seatNumber);
            _isWaitingResponse = false;
        }
    }
}