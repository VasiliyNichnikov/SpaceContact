namespace Core.User
{
    public sealed class ServerUserFactory
    {
        private const string DefaultName = "None";

        private readonly UsersColorController _colorController;
        private readonly UsersSeatsController _seatsController;
        
        public ServerUserFactory(
            UsersColorController colorController,
            UsersSeatsController seatsController)
        {
            _colorController = colorController;
            _seatsController = seatsController;
        }
        
        public ServerUser Create(
            ulong ownerClientId, 
            ulong localClientId, 
            bool isHost)
        {
            var colorId = _colorController.GetFreeColor();
            var seatNumber = _seatsController.GetFreeSeatNumber();
            
            return new ServerUser(
                DefaultName,
                seatNumber,
                colorId,
                ownerClientId,
                localClientId,
                isHost);
        }
    }
}