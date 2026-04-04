using Core.User.Dto.States;

namespace Core.User
{
    public sealed class ClientUserFactory
    {
        private readonly UsersColorController _colorController;
        private readonly UsersSeatsController _seatsController;
        
        public ClientUserFactory(
            UsersColorController colorController,
            UsersSeatsController seatsController)
        {
            _colorController = colorController;
            _seatsController = seatsController;
        }
        
        public ClientUser Create(
            ulong localClientId, 
            UserStateData state)
        {
            return new ClientUser(
                localClientId,
                state,
                _colorController,
                _seatsController);
        }
    }
}