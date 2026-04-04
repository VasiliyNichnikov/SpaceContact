using Core.User.Dto.States;

namespace Core.User
{
    public interface IServerUser : IUser
    {
        UserStateData ToState();
        
        void SetName(string name);
        
        void SetColorId(int colorId);
        
        void SetSeatNumber(int seatNumber);
    }
}