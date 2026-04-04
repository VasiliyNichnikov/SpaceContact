using System;
using Core.User.Dto.States;

namespace Core.User
{
    public sealed class ServerUser : IServerUser
    {
        private readonly ulong _localClientId;
        
        public ServerUser(
            string name,
            int seatNumber,
            int colorId,
            ulong ownerClientId,
            ulong localClientId,
            bool isHost)
        {
            Name = name;
            SeatNumber = seatNumber;
            ClientId = ownerClientId;
            IsOwnerLobby = isHost;
            _localClientId = localClientId;
            ColorId = colorId;
        }
        
        public ulong ClientId { get; }
        
        public string Name { get; private set; }
        
        public int ColorId { get; private set; }
        
        public int SeatNumber { get; private set; }
        
        public bool IsCurrentPlayer => 
            _localClientId == ClientId;

        public bool IsOwnerLobby { get; }
        
        public event Action? Changed;

        void IServerUser.SetName(string name)
        {
            Name = name;
            Changed?.Invoke();
        }

        void IServerUser.SetColorId(int colorId)
        {
            ColorId = colorId;
            Changed?.Invoke();
        }

        void IServerUser.SetSeatNumber(int seatNumber)
        {
            SeatNumber = seatNumber;
            Changed?.Invoke();
        }
        
        UserStateData IServerUser.ToState()
        {
            return new UserStateData
            {
                UserId = ClientId,
                ColorId = ColorId,
                IsOwnerLobby = IsOwnerLobby,
                Name = Name,
                SeatNumber = SeatNumber,
            };
        }
    }
}